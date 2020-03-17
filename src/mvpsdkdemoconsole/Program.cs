using System;
using System.IO;

using System.Text;

using System.Threading.Tasks;

using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNetEnv;

namespace mvpsdkdemoconsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load the .env file environment variables
            Env.Load();

            // Set blob client options for more retries:
            var options = new BlobClientOptions()
            {
                Retry = {
                    MaxRetries = 10,
                    Delay = TimeSpan.FromSeconds(3)
                },
                Diagnostics = {
                    IsLoggingEnabled = false
                }
            };

            // Add our own custom policy to the HTTP pipeline:
            options.AddPolicy(new SimpleTracingPolicy(), HttpPipelinePosition.PerCall);

            // Get storage account blob url from settings
            var blobServiceUri = new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_URL"));

            // Create a BlobServiceClient to our Storage account using DefaultAzureCredentials & our HTTP pipeline options:
            var serviceClient = new BlobServiceClient(blobServiceUri, new DefaultAzureCredential(), options);

            // Create a container in our Storage account:
            var containerClient = serviceClient.GetBlobContainerClient("blobs");
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Upload a blob to container:
            var blobClient = containerClient.GetBlobClient("blob.txt");
            var blob = await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("https://aka.ms/azsdkmvps")), overwrite: true);

            // Download the blob
            var blobDownload = await blobClient.DownloadAsync();
            using var blobStreamReader = new StreamReader(blobDownload.Value.Content);

            // Write the blob contents
            Console.WriteLine(blobStreamReader.ReadToEnd());
        }
    }

    public class SimpleTracingPolicy : HttpPipelinePolicy
    {
        public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            Console.WriteLine($">> Request: {message.Request.Method} {message.Request.Uri}");
            await ProcessNextAsync(message, pipeline);
            Console.WriteLine($">> Response: {message.Response.Status} from {message.Request.Method} {message.Request.Uri}\n");
        }

        #region public override void Process
        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            Console.WriteLine($">> Request: {message.Request.Uri}");
            ProcessNext(message, pipeline);
            Console.WriteLine($">> Response: {message.Response.Status} {message.Request.Method} {message.Request.Uri}");
        }
        #endregion public override void Process
    }
}

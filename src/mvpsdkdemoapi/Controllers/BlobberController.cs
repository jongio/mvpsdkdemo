
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace mvpsdkdemoapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobberController : ControllerBase
    {
        private readonly BlobServiceClient blobClient;

        public BlobberController(BlobServiceClient blobClient)
        {
            this.blobClient = blobClient;
        }

        // GET: api/Blobber
        [HttpGet]
        public async Task<dynamic> Get()
        {
            // Create a container in our Storage account:
            var containerClient = this.blobClient.GetBlobContainerClient("blobs");
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Upload a blob to our container:
            var blobClient = containerClient.GetBlobClient("blob.txt");
            var blob = await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("Click here to view our Azure SDK MVPs: https://aka.ms/azsdkmvps")), overwrite: true);

            // Return the blob contents:
            var blobDownload = await blobClient.DownloadAsync();
            using var blobStreamReader = new StreamReader(blobDownload.Value.Content);
            return new {Content = blobStreamReader.ReadToEnd()};
        }
    }
}

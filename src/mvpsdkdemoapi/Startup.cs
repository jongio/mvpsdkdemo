using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Azure.Core;

using Azure.Identity;

namespace mvpsdkdemoapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<SimpleTracingPolicy>();

            services.AddAzureClients(builder =>
            {
                
                builder.AddBlobServiceClient(new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_URL")))
                    .ConfigureOptions((options, provider) =>
                    {
                        options.Retry.MaxRetries = 10;
                        options.Retry.Delay = TimeSpan.FromSeconds(3);
                        options.Diagnostics.IsLoggingEnabled = true;
                        options.AddPolicy(provider.GetService<SimpleTracingPolicy>(), HttpPipelinePosition.PerCall);
                    }).WithCredential(new DefaultAzureCredential());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UploaderFunctions
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var file = req.Form.Files["file"];

            var connectionString = Environment.GetEnvironmentVariable("MediaStorage");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("media");

            await containerClient.CreateIfNotExistsAsync();

            await containerClient.UploadBlobAsync($"{Guid.NewGuid()}_{file.FileName}", file.OpenReadStream());

            return new OkObjectResult($"'{file.Name}' has {file.Length}");
        }
    }
}


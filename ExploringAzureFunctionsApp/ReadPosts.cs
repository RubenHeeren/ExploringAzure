using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ExploringAzureFunctionsApp;

public class ReadPosts
{
    private readonly ILogger<ReadPosts> _logger;

    public ReadPosts(ILogger<ReadPosts> log)
    {
        _logger = log;
    }

    [FunctionName("ReadPosts")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Read posts" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
        HttpRequest httpRequest,
        [CosmosDB(
            databaseName: "ExploringAzureDB",
            collectionName: "ExploringAzureContainer",
            SqlQuery = "SELECT * FROM c",
            ConnectionStringSetting = "CosmosDbConnectionString")] IEnumerable<Document> posts
        )
    {
        _logger.LogInformation($"C# HTTP trigger function processed a request. Function name: {nameof(ReadPosts)}");

        if (posts is null)
        {
            return new NotFoundResult();
        }

        foreach (var post in posts)
        {
            _logger.LogInformation(post.GetPropertyValue<string>("title"));
        }

        return new OkObjectResult(posts);
    }
}

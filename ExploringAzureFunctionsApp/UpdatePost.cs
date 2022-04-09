using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ExploringAzureFunctionsApp;

public class UpdatePost
{
    private readonly ILogger<UpdatePost> _logger;

    public UpdatePost(ILogger<UpdatePost> log)
    {
        _logger = log;
    }

    [FunctionName("UpdatePost")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Update post" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **ID** parameter")]
    [OpenApiParameter(name: "new-title", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **New Title** parameter")]
    [OpenApiParameter(name: "new-content", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **New Content** parameter")]
    [OpenApiParameter(name: "new-published", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "The **New Published** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)]
        HttpRequest httpRequest,
        [CosmosDB(
            databaseName: "ExploringAzureDB",
            collectionName: "ExploringAzureContainer",
            Id = "{Query.id}",
            PartitionKey = "{Query.id}",
            ConnectionStringSetting = "CosmosDbConnectionString")] Document document,
        [CosmosDB(
            databaseName: "ExploringAzureDB",
            collectionName: "ExploringAzureContainer",
            ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient client
        )
    {
        _logger.LogInformation($"C# HTTP trigger function processed a request. Function name: {nameof(UpdatePost)}");

        string id = httpRequest.Query["id"];

        if (document == null || string.IsNullOrEmpty(id))
        {
            return new BadRequestResult();
        }

        string newTitle = httpRequest.Query["new-title"];
        string newContent = httpRequest.Query["new-content"];
        bool newPublished = bool.Parse(httpRequest.Query["new-published"]);

        document.SetPropertyValue("title", newTitle);
        document.SetPropertyValue("content", newContent);
        document.SetPropertyValue("isPublished", newPublished);

        await client.ReplaceDocumentAsync(document.SelfLink, document);

        return new OkResult();
    }
}

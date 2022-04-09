using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ExploringAzureFunctionsApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ExploringAzureFunctionsApp;

public class CreatePost
{
    private readonly ILogger<CreatePost> _logger;

    public CreatePost(ILogger<CreatePost> log)
    {
        _logger = log;
    }

    [FunctionName("CreatePost")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Create post" })]
    [OpenApiParameter(name: "title", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Title** parameter")]
    [OpenApiParameter(name: "content", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Content** parameter")]
    [OpenApiParameter(name: "published", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "The **Published** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [CosmosDB(
            databaseName: "ExploringAzureDB",
            collectionName: "ExploringAzureContainer",
            ConnectionStringSetting = "CosmosDbConnectionString")]
        IAsyncCollector<dynamic> documentsOut)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string title = req.Query["title"];
        string content = req.Query["content"];
        bool published = bool.Parse(req.Query["published"]);        

        Post postToCreate = new()
        {
            // Create a random ID.
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Content = content,
            Published = published
        };

        // Add a JSON document to the output container.
        await documentsOut.AddAsync(postToCreate);

        string responseMessage = "Function triggered successfully. If the data was clean, the post has been created.";

        return new OkObjectResult(responseMessage);
    }
}

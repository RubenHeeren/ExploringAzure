using Newtonsoft.Json;

namespace ExploringAzureFunctionsApp.Models;
internal class Post
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    [JsonProperty(PropertyName = "content")]
    public string Content { get; set; }

    [JsonProperty(PropertyName = "isPublished")]
    public bool Published { get; set; }
}

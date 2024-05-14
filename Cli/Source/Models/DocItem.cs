using Newtonsoft.Json;

namespace DocumentationWarning.Models;

public class DocItem
{
    [JsonProperty("id")]
    public required string Id;

    [JsonProperty("name")]
    public required string Name;
}

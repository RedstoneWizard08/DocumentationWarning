using Newtonsoft.Json;

namespace Extractor;

public class DocItem
{
    [JsonProperty("id")]
    public required string Id;

    [JsonProperty("name")]
    public required string Name;
}

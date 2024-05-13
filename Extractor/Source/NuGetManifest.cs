using System.Collections.Generic;
using Newtonsoft.Json;

namespace Extractor;

[JsonObject]
public class NuGetManifest
{
    [JsonProperty]
    public List<string> versions = [];
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Extractor;

[JsonObject]
public class NugetManifest
{
    [JsonProperty]
    public List<string> versions = [];
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace DocumentationWarning.Models;

[JsonObject]
public class NuGetManifest
{
    [JsonProperty]
    public List<string> versions = [];
}

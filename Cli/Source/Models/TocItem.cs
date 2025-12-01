using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DocumentationWarning.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class TocItem {
    public required string Name;
    public required string Href;
    public required string TopicHref;
    public required string TopicUid;
    public required string Type;

    public List<TocItem> Items = [];
}

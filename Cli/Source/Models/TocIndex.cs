using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DocumentationWarning.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class TocIndex {
    public required List<TocItem> Items;
    public required string MemberLayout;
}

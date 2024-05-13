using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Docfx.Build.Engine;
using Docfx.Common;
using Docfx.MarkdigEngine.Extensions;
using Docfx.Plugins;
using Extractor.Config;

namespace Extractor.Steps;

public sealed class FixYaml : Step
{
    public override async Task Run(ProjectConfig config)
    {
        var startRegex = new Regex($@"^{config.Game.Namespace}\.");
        var commentRegex = new Regex($@"^([^:]+):{config.Game.Namespace}\.");
        var path = Path.Join(config.DocDir, "_site", "xrefmap.yml");
        var map = YamlUtility.Deserialize<XRefMap>(path);
        var refs = new List<XRefSpec>();

        foreach (var item in map.References) {
            if (item.Uid.StartsWith(config.Game.Namespace) && item.Uid != config.Game.Namespace) {
                var alt = item;

                alt.Uid = item.Uid.ReplaceRegex(startRegex, "");

                if (map.References.Find(v => v.Uid == alt.Uid) != null)
                    continue;
                
                if (alt.ContainsKey("fullName")) {
                    alt["fullName"] = item["fullName"].ToString().ReplaceRegex(startRegex, "");
                }

                refs.Add(alt);
            }
        }

        map.References.AddRange(refs);

        var stream = File.Open(path, FileMode.Create);
        var writer = new StreamWriter(stream);

        await writer.WriteLineAsync("### " + YamlMime.XRefMap);
        
        YamlUtility.Serialize(writer, map);

        await writer.FlushAsync();
        writer.Close();
        await writer.DisposeAsync();
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Docfx.Build.Engine;
using Docfx.Common;
using Docfx.MarkdigEngine.Extensions;
using Docfx.Plugins;
using Docfx.YamlSerialization;
using Newtonsoft.Json;

namespace Extractor.Steps;

public sealed class FixYaml : Step
{
    public static Regex StartRegex = new Regex(@"^DefaultNamespace\.");
    public static Regex CommentRegex = new Regex(@"^([^:]+):DefaultNamespace\.");

    public override async Task Run()
    {
        var path = Path.Join("..", "_site", "xrefmap.yml");
        var map = YamlUtility.Deserialize<XRefMap>(path);
        var refs = new List<XRefSpec>();

        foreach (var item in map.References) {
            if (item.Uid.StartsWith(Config.Namespace) && item.Uid != Config.Namespace) {
                var alt = item;

                alt.Uid = item.Uid.ReplaceRegex(StartRegex, "");

                if (map.References.Find(v => v.Uid == alt.Uid) != null)
                    continue;
                
                if (alt.ContainsKey("fullName")) {
                    alt["fullName"] = item["fullName"].ToString().ReplaceRegex(StartRegex, "");
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

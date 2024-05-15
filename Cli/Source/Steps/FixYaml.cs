using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Docfx.Build.Engine;
using Docfx.Common;
using Docfx.MarkdigEngine.Extensions;
using Docfx.Plugins;
using DocumentationWarning.Config;
using DocumentationWarning.Util;
using ShellProgressBar;

namespace DocumentationWarning.Steps;

public sealed class FixYaml : Step
{
    public override async Task Run(ProjectConfig config)
    {
        var root = await ConfigHelper.GetRootConfig();
        var urlPrefix = $"{root.Site}/docs/{config.Game.Id}/";
        var startRegex = new Regex($@"^{config.Game.Namespace}\.");
        var commentRegex = new Regex($@"^([^:]+):{config.Game.Namespace}\.");
        var path = Path.Join(config.DocDir, "_site", "xrefmap.yml");
        var map = YamlUtility.Deserialize<XRefMap>(path);
        var refs = new List<XRefSpec>();
        var total = map.References.Count;

        using (var bar = new ProgressBar(total, "Fixing xrefmap..."))
        {
            var prog = bar.AsProgress<float>();

            for (var i = 0; i < total; i++)
            {
                prog.Report(i / total);

                var item = map.References[i];

                item.Href = urlPrefix + item.Href;

                map.References[i] = item;

                if (item.Uid.StartsWith(config.Game.Namespace) && item.Uid != config.Game.Namespace)
                {
                    var alt = item;

                    alt.Uid = item.Uid.ReplaceRegex(startRegex, "");

                    if (map.References.Find(v => v.Uid == alt.Uid) != null)
                        continue;

                    if (alt.ContainsKey("fullName"))
                    {
                        alt["fullName"] = item["fullName"].ToString().ReplaceRegex(startRegex, "");
                    }

                    refs.Add(alt);
                }
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

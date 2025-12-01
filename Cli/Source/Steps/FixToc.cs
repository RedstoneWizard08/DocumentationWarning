using System.IO;
using System.Threading.Tasks;
using Docfx.Build.TableOfContents;
using Docfx.Common;
using Docfx.DataContracts.Common;
using DocumentationWarning.Config;
using DocumentationWarning.Models;
using DocumentationWarning.Util;
using Newtonsoft.Json;

namespace DocumentationWarning.Steps;

public sealed class FixToc : Step {
    protected override async Task Run(ProjectConfig config) {
        "Reading Table of Contents...".LogInfo(this);

        var siteDir = Path.Join(config.DocDir, "_site");
        var tocFile = Path.Join(siteDir, "api", "toc.json");
        var toc = JsonConvert.DeserializeObject<TocIndex>(await File.ReadAllTextAsync(tocFile));

        if (toc == null) {
            "Failed to read Table of Contents!".LogCritical(this);
            return;
        }

        "Fixing Table of Contents...".LogInfo(this);

        foreach (var item in toc.Items) {
            if (item.TopicUid == config.Game.Namespace && item.Name == "") {
                item.Name = config.Game.Namespace;
            }
        }

        "Saving Table of Contents...".LogInfo(this);

        File.WriteAllText(tocFile, JsonConvert.SerializeObject(toc));
    }
}

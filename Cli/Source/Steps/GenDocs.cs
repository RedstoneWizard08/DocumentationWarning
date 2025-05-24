using System.Threading.Tasks;
using System.IO;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class GenDocs : Step {
    protected override async Task Run(ProjectConfig config) {
        await ResExtractor.Extract(config.DocDir, config);

        Directory.CreateDirectory(config.DocDir);
        File.Copy(config.Game.BannerPath(config), Path.Join(config.DocDir, config.Game.Banner));
        File.Copy(config.Game.IconPath(config), Path.Join(config.DocDir, config.Game.Icon));
    }
}
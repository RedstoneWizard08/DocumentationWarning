using System.Threading.Tasks;
using System.IO;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class GenDocs : Step
{
    public override async Task Run(ProjectConfig config)
    {
        new ResExtractor().Extract(config.DocDir, config);

        if (!Directory.Exists(config.DocDir))
        {
            Directory.CreateDirectory(config.DocDir);
        }

        File.Copy(config.Game.BannerPath(config), Path.Join(config.DocDir, config.Game.Banner));
        File.Copy(config.Game.IconPath(config), Path.Join(config.DocDir, config.Game.Icon));
    }
}

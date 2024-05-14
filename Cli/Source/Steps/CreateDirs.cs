using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;

namespace DocumentationWarning.Steps;

public class CreateDirs : Step
{
    public override async Task Run(ProjectConfig config)
    {
        if (!Directory.Exists(config.TempDir))
        {
            Directory.CreateDirectory(config.TempDir);
        }

        if (!Directory.Exists(config.AsmDir))
        {
            Directory.CreateDirectory(config.AsmDir);
        }

        if (!Directory.Exists(config.PkgDir))
        {
            Directory.CreateDirectory(config.PkgDir);
        }
    }
}

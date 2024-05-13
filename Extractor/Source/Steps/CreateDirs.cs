using System.IO;
using System.Threading.Tasks;

namespace Extractor.Steps;

public class CreateDirs : Step
{
    public override async Task Run()
    {
        if (!Directory.Exists(Config.TempDir))
        {
            Directory.CreateDirectory(Config.TempDir);
        }

        if (!Directory.Exists(Config.AsmDir))
        {
            Directory.CreateDirectory(Config.AsmDir);
        }

        if (!Directory.Exists(Config.PkgDir))
        {
            Directory.CreateDirectory(Config.PkgDir);
        }
    }
}

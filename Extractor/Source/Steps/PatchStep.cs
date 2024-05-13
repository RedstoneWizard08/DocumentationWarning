using System.IO;
using System.Threading.Tasks;

namespace Extractor.Steps;

public sealed class PatchStep : Step
{
    public override async Task Run()
    {
        foreach (var file in Config.Patches)
        {
            await new Patcher().Run(file);
        }

        foreach (var dir in Config.RemovedDirs) {
            var real = Path.Join(Config.OutDir, "Assembly-CSharp", dir);

            Directory.Delete(real, true);
        }
    }
}

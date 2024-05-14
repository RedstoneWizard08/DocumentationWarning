using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Processors;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class PatchStep : Step
{
    public override async Task Run(ProjectConfig config)
    {
        foreach (var patch in PatchScanner.ScanPre(config))
        {
            "Applying patch: {}".LogInfo(this, patch);

            await new Patcher().Run(config, patch);
        }

        foreach (var (key, dirs) in config.Remove)
        {
            foreach (var dir in dirs)
            {
                var real = Path.Join(config.OutDir, key, dir);

                Directory.Delete(real, true);
            }
        }
    }
}

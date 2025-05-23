using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Processors;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class PatchStep(Func<ProjectConfig, string>? path) : Step {
    private readonly Func<ProjectConfig, string> path = path ?? ((cfg) => cfg.OutDir);

    public override async Task Run(ProjectConfig config) {
        foreach (var patch in PatchScanner.ScanPre(config)) {
            "Applying patch: {}".LogInfo(this, patch);

            await new Patcher().Run(config, patch);
        }

        foreach (var (key, dirs) in config.Remove) {
            foreach (var real in dirs.Select(dir => Path.Join(path(config), key, dir))) {
                if (!Directory.Exists(real)) {
                    $"Directory marked for removal does not exist: {real}".LogWarn(this);
                    continue;
                }

                Directory.Delete(real, true);
            }
        }
    }
}
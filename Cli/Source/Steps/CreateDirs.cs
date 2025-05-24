using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;

namespace DocumentationWarning.Steps;

public class CreateDirs : Step {
    protected override async Task Run(ProjectConfig config) {
        Directory.CreateDirectory(config.TempDir);
        Directory.CreateDirectory(config.AsmDir);
        Directory.CreateDirectory(config.PkgDir);
    }
}
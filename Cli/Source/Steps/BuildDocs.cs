using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class BuildDocs : Step {
    protected override async Task Run(ProjectConfig config) {
        await CmdHelper.Run(config.DocDir, "docfx");
    }
}
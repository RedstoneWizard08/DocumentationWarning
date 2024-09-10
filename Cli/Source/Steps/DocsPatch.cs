using System;
using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Processors;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class DocsPatch(Func<ProjectConfig, string>? root) : Step
{
    private readonly Func<ProjectConfig, string> root = root ?? ((cfg) => cfg.OutDir);

    public override async Task Run(ProjectConfig config)
    {
        var path = Path.Join(root(config), "Assembly-CSharp");
        var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var data = await File.ReadAllTextAsync(file);

            if (data.Contains("namespace ")) continue;

            data = $"namespace {config.Game.Namespace};\n\n" + data;

            await File.WriteAllTextAsync(file, data);
        }

        foreach (var patch in PatchScanner.ScanPost(config))
        {
            "Applying patch: {}".LogInfo(this, patch);

            await new Patcher().Run(config, patch);
        }
    }
}

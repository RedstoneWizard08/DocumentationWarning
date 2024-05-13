using System.IO;
using System.Threading.Tasks;
using Extractor.Config;

namespace Extractor.Steps;

public sealed class DocsPatch : Step
{
    public override async Task Run(ProjectConfig config)
    {
        var path = Path.Join(config.OutDir, "Assembly-CSharp");
        var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var data = await File.ReadAllTextAsync(file);

            if (data.Contains("namespace ")) continue;

            data = $"namespace {config.Game.Namespace};\n\n" + data;

            await File.WriteAllTextAsync(file, data);
        }

        foreach (var patch in PatchScanner.ScanPost(config)) {
            "Applying patch: {}".LogInfo(this, patch);

            await new Patcher().Run(config, patch);
        }
    }
}

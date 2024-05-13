using System.IO;
using System.Threading.Tasks;

namespace Extractor.Steps;

public sealed class DocsPatch : Step
{
    public override async Task Run()
    {
        var path = Path.Join(Config.OutDir, "Assembly-CSharp");
        var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var data = await File.ReadAllTextAsync(file);

            if (data.Contains("namespace ")) continue;

            data = $"namespace {Config.Namespace};\n\n" + data;

            await File.WriteAllTextAsync(file, data);
        }

        foreach (var patch in await PatchScanner.ScanPost()) {
            await new Patcher().Run(patch);
        }
    }
}

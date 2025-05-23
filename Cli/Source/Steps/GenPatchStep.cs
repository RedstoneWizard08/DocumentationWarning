using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public class GenPatchStep : Step {
    public override async Task Run(ProjectConfig config) {
        var outPath = config.OutDirStr;
        var tempPath = Path.Join(config.TempDirStr, "PatchSource");
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var files = Directory.GetFiles(Path.Join(config.ProjectDir, "Patches", "Post"));
        var filename = $"{files.Length:0000}-{date}-Generated.patch";
        var patch = Path.Join(config.ProjectDir, "Patches", "Post", filename);

        File.Copy(
            Path.Join(config.OutDir, "Project.sln"),
            Path.Join(config.TempDir, "PatchSource", "Project.sln"),
            true
        );

        var data = await CmdHelper.Run(
            config.ProjectDir,
            "diff",
            false,
            "-Naur",
            "-x",
            "*.csproj",
            "-x",
            "obj",
            "-x",
            "bin",
            tempPath,
            outPath
        );

        foreach (var dir in Directory.GetDirectories(config.OutDir)) {
            var dirFiles = Directory.GetFiles(dir);
            var csproj = dirFiles.First(it => it.EndsWith(".csproj"));
            var csprojName = Path.GetFileName(csproj);
            var dirName = Path.GetFileName(dir);
            var newDir = Path.Join(config.ProjectDir, "Patches", "Replace", dirName);
            var newCsproj = Path.Join(newDir, csprojName);

            if (!Directory.Exists(newDir)) {
                Directory.CreateDirectory(newDir);
            }

            File.Copy(csproj, newCsproj, true);
        }

        await File.WriteAllTextAsync(patch, data);
    }
}
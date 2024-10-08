using System;
using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Util;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;

namespace DocumentationWarning.Steps;

public sealed class DecompileStep(Func<ProjectConfig, string>? path) : Step
{
    private readonly Func<ProjectConfig, string> path = path ?? ((cfg) => cfg.OutDir);

    public override async Task Run(ProjectConfig config)
    {
        foreach (var file in config.Assemblies)
        {
            var path = Path.Join(config.AsmDir, file);

            "Decompiling {}...".LogInfo(this, file);

            await DecompileProject(path, config);
        }
    }

    private DecompilerSettings GetSettings(PEFile module)
    {
        return new DecompilerSettings(LanguageVersion.Latest)
        {
            ThrowOnAssemblyResolveErrors = false,
            RemoveDeadCode = false,
            RemoveDeadStores = false,
            UseSdkStyleProjectFormat = WholeProjectDecompiler.CanUseSdkStyleProjectFormat(module),
            UseNestedDirectoriesForNamespaces = true,
        };
    }

    private async Task<ProjectId> DecompileProject(string assembly, ProjectConfig config)
    {
        var baseName = Path.GetFileNameWithoutExtension(assembly);
        var projectDir = Path.Join(path(config), baseName);

        if (Directory.Exists(projectDir))
        {
            Directory.Delete(projectDir, true);
        }

        if (!Directory.Exists(projectDir))
        {
            Directory.CreateDirectory(projectDir);
        }

        var projectFile = Path.Join(projectDir, baseName + ".csproj");
        var module = new PEFile(assembly);
        var resolver = new UniversalAssemblyResolver(assembly, false, module.Metadata.DetectTargetFrameworkId());
        var decompiler = new WholeProjectDecompiler(GetSettings(module), resolver, resolver, null);

        using (var projectFileWriter = new StreamWriter(File.OpenWrite(projectFile)))
            return decompiler.DecompileProject(module, Path.GetDirectoryName(projectFile), projectFileWriter);
    }
}

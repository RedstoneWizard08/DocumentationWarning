using System.IO;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;

namespace Extractor.Steps;

public sealed class Decompile : Step
{
    public override async Task Run()
    {
        foreach (var file in Config.Assemblies)
        {
            var path = Path.Join(Config.AsmDir, file);

            "Decompiling {}...".LogInfo(this, file);

            await DecompileProject(path);
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

    private async Task<ProjectId> DecompileProject(string assembly)
    {
        var baseName = Path.GetFileNameWithoutExtension(assembly);
        var projectDir = Path.Join(Config.OutDir, baseName);

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

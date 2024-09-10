using System.IO;
using System.Threading.Tasks;
using CommandLine;
using DocumentationWarning.Steps;
using DocumentationWarning.Util;

namespace DocumentationWarning.Commands;

public sealed class CreatePatch : Command<CreatePatch.Options>
{
    [Verb("createPatch", HelpText = "Create a patch file.")]
    public class Options
    {
        [Value(0, MetaName = "Project", Required = true, HelpText = "The project to create a patch for.")]
        public required string Project { get; set; }
    }

    internal static Step[] DecompileSteps = [
        new CreateDirs(),
        new DownloadPkgs(),
        new ExtractAsm(),
        new DecompileStep((cfg) => Path.Join(cfg.TempDir, "PatchSource")),
        new PatchStep((cfg) => Path.Join(cfg.TempDir, "PatchSource")),
        new DocsPatch((cfg) => Path.Join(cfg.TempDir, "PatchSource")),
        new GenSolution((cfg) => Path.Join(cfg.TempDir, "PatchSource")),
        new GenPatchStep(),
    ];

    public override async Task Execute(Options options)
    {
        var cfg = GetConfig(options.Project);

        if (cfg == null)
        {
            "Cannot find project: {}".LogCritical(this, options.Project);

            return;
        }
        
        await Step.Run(cfg, DecompileSteps);
    }
}

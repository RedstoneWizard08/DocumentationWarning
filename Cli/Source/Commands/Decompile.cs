using System.Threading.Tasks;
using CommandLine;
using DocumentationWarning.Steps;
using DocumentationWarning.Util;

namespace DocumentationWarning.Commands;

public sealed class Decompile : Command<Decompile.Options> {
    [Verb("decompile", HelpText = "Decompile assemblies.")]
    public class Options {
        [Option('a', "all", Default = true, HelpText = "Decompile all projects.")]
        public bool? All { get; set; } = true;

        [Value(0, MetaName = "Project", Required = false, HelpText = "The project to decompile.")]
        public string? Project { get; set; } = null;
    }

    private static Step[] Steps = [
        new CreateDirs(),
        new DownloadPkgs(),
        new ExtractAsm(),
        new DecompileStep(null),
        new PatchStep(null),
        new DocsPatch(null),
        new GenSolution(null),
    ];

    public override async Task Execute(Options options) {
        if (options.Project != null) {
            var cfg = GetConfig(options.Project);

            if (cfg == null) {
                "Cannot find project: {}".LogCritical(this, options.Project);

                return;
            }

            await Step.Run(cfg, Steps);

            return;
        }

        if (options.All == true) {
            foreach (var config in Configs) {
                await Step.Run(config, Steps);
            }
        }
    }
}
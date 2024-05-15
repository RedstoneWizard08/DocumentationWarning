using System.Threading.Tasks;
using CommandLine;
using DocumentationWarning.Steps;
using DocumentationWarning.Util;

namespace DocumentationWarning.Commands;

public sealed class Build: Command<Build.Options> {
    [Verb("build", isDefault: true, HelpText = "Build the documentation site.")]
    public class Options
    {
        [Option('a', "all", Default = true, HelpText = "Build all projects.")]
        public bool? All { get; set; } = true;

        [Option('d', "decompile", Default = true, HelpText = "Run decompile when building.")]
        public bool? Decompile { get; set; } = true;

        [Option('g', "generate", Default = true, HelpText = "Run generate when building.")]
        public bool? Generate { get; set; } = true;

        [Value(0, MetaName = "Project", Required = false, HelpText = "The project to build.")]
        public string? Project { get; set; } = null;
    }

    internal static Step[] Steps = [
        new BuildDocs(),
        new FixYaml(),
        new CopyDocs(),
    ];

    public override async Task Execute(Options options)
    {
        if (options.Decompile == true) {
            await new Decompile().Run(new Decompile.Options {
                All = options.All,
                Project = options.Project,
            });
        }

        if (options.Generate == true) {
            await new Generate().Run(new Generate.Options {
                All = options.All,
                Project = options.Project,
            });
        }

        if (options.Project != null)
        {
            var cfg = GetConfig(options.Project);

            if (cfg == null)
            {
                "Cannot find project: {}".LogCritical(this, options.Project);

                return;
            }

            await Step.Run(cfg, Steps);

            return;
        }

        if (options.All == true)
        {
            foreach (var config in Configs)
            {
                await Step.Run(config, Steps);
            }
        }
    }
}

using System.Threading.Tasks;
using CommandLine;
using DocumentationWarning.Steps;
using DocumentationWarning.Util;

namespace DocumentationWarning.Commands;

public sealed class Generate : Command<Generate.Options>
{
    [Verb("generate", HelpText = "Generate DocFx files.")]
    public class Options
    {
        [Option('a', "all", Default = true, HelpText = "Run for all projects.")]
        public bool? All { get; set; } = true;

        [Value(0, MetaName = "Project", Required = false, HelpText = "The project to generate for.")]
        public string? Project { get; set; }
    }

    public override async Task Execute(Options options)
    {
        if (options.Project != null)
        {
            var cfg = GetConfig(options.Project);

            if (cfg == null)
            {
                "Cannot find project: {}".LogCritical(this, options.Project);

                return;
            }

            await Step.Run(cfg, new GenDocs());

            return;
        }

        if (options.All == true)
        {
            foreach (var config in Configs)
            {
                await Step.Run(config, new GenDocs());
            }
        }
    }
}

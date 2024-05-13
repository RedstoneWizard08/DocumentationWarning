using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Extractor.Config;
using Extractor.Steps;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Extractor;

public class Program : WithLogger
{
    internal static readonly ILoggerFactory Factory = LoggerFactory.Create(b => b.AddConsole());
    public static readonly List<ProjectConfig> Configs = [];

    private static readonly Step[] Steps = [
        new CreateDirs(),
        new DownloadPkgs(),
        new ExtractAsm(),
        new Decompile(),
        new PatchStep(),
        new DocsPatch(),
        new GenSolution(),
        new BuildDocs(),
        new FixYaml(),
    ];

    public static void Main() => new Program().Start();

    public void Start()
    {
        "Starting Extractor...".LogInfo(this);

        Run().GetAwaiter().GetResult();
    }

    public async Task Run()
    {
        var projectsData = await File.ReadAllTextAsync("projects.json");
        var projects = JsonConvert.DeserializeObject<List<string>>(projectsData)!;

        foreach (var proj in projects)
        {
            Configs.Add(await ProjectConfig.Load(proj, Path.Join(proj, "config.json")));
        }

        foreach (var config in Configs)
        {
            "Running for project: {}".LogInfo(this, config.Game.Name);

            foreach (var step in Steps)
            {
                "Running step {}...".LogInfo(this, step.GetType().FullName);

                await step.Run(config);
            }
        }
    }
}

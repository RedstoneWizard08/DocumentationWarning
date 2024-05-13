using System.Threading.Tasks;
using Extractor.Steps;
using Microsoft.Extensions.Logging;

namespace Extractor;

public class Program: WithLogger
{
    internal static readonly ILoggerFactory Factory = LoggerFactory.Create(b => b.AddConsole());

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
        foreach (var step in Steps)
        {
            "Running step {}...".LogInfo(this, step.GetType().FullName);

            await step.Run();
        }
    }
}

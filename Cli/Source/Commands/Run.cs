using System.Threading.Tasks;
using CommandLine;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentationWarning.Commands;

public sealed class Run: Command<Run.Options> {
    [Verb("run", HelpText = "Run the documentation server.")]
    public class Options
    {
        [Option('p', "port", Default = 4000, HelpText = "The port to run on.")]
        public int Port { get; set; } = 4000;

        [Option('g', "generate", Default = true, HelpText = "Run generate when building.")]
        public bool? Generate { get; set; } = true;

        [Option('d', "decompile", Default = true, HelpText = "Run decompile when building.")]
        public bool? Decompile { get; set; } = true;
    }

    public override async Task Execute(Options options)
    {
        if (options.Decompile == true) {
            await new Decompile().Run(new Decompile.Options {
                All = true,
            });
        }

        if (options.Generate == true) {
            await new Generate().Run(new Generate.Options {
                All = true,
            });
        }

        await new Build().Run(new Build.Options {
            All = true,
            Decompile = false,
            Generate = false,
        });

        var builder = WebApplication.CreateBuilder();
        var host = WebHost.CreateDefaultBuilder();

        builder.Services.AddDirectoryBrowser();
        host.UseWebRoot(".");
        host.UseUrls($"http://0.0.0.0:{options.Port}");

        var app = builder.Build();

        app.UseFileServer(enableDirectoryBrowsing: true);
        app.Run();
    }
}

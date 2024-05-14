using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Docfx.Common;
using DocumentationWarning.Config;
using DocumentationWarning.Models;
using DocumentationWarning.Util;
using Newtonsoft.Json;
using Sharprompt;

namespace DocumentationWarning.Commands;

public sealed class Init: Command<Init.Options> {
    [Verb("init", HelpText = "Initialize support for a new game.")]
    public class Options
    {
        [Value(0, MetaName = "Id", Required = false, HelpText = "The game's ID (internal name).")]
        public string? Id { get; set; } = null;
    }

    public override async Task Execute(Options options)
    {
        var unityVersions = await Http.GetManifest("UnityEngine.Modules", DepSource.BaGet);
        var id = options.Id ?? Prompt.Input<string>("What is the game's internal name? (Directory Name)");
        var name = Prompt.Input<string>("What is the game's name?");
        var pub = Prompt.Input<string>("Who published the game?");
        var pkg = Prompt.Input<string>("What is the package for the game's assemblies?");
        var src = Prompt.Select("Where is this package?", ["NuGet", "BaGet"]);
        var ns = Prompt.Input<string>("What namespace should non-namespaced classes be relocated to?");
        var unity = Prompt.Select("What Unity version was the game made with?", unityVersions.versions);
        var framework = Prompt.Input<string>("What .NET framework was the game built with?");
        var unityFramework = framework == "netstandard2.1" ? "netstandard2.0" : framework;
        var steam = Prompt.Input<string>("What is the game's Steam page?");
        var hasWebsite = Prompt.Confirm("Does the game have a website?", true);
        var website = hasWebsite ? Prompt.Input<string>("What is the game's website?") : null;
        var hasThunderstore = Prompt.Confirm("Does the game have a Thunderstore community?", true);
        var thunderstore = hasThunderstore ? Prompt.Input<string>("What is the game's Thunderstore page?") : null;

        var it = new ProjectConfig() {
            Game = new GameConfig() {
                Id = options.Id!,
                Name = name,
                Package = pkg,
                Publisher = pub,
                Banner = "",
                Icon = "",
                Namespace = ns,
                Source = src.Parse(),
                Unity = unity,
            },

            Framework = new FrameworkConfig() {
                Package = framework,
                Unity = unityFramework,
            },

            Urls = new UrlConfig() {
                Steam = steam,
                Thunderstore = thunderstore,
                Website = website,
            },
        };

        it.Dependencies.Add(new Dependency() {
            Name = "UnityEngine.Modules",
            Version = unity,
            Source = DepSource.BaGet,
        });

        var dir = Path.Join("Games", id);

        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }

        var path = Path.Join(dir, "config.json");

        await File.WriteAllTextAsync(path, it.ToJsonString(Formatting.Indented));
    }
}

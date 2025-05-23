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

public sealed class Init : Command<Init.Options> {
    [Verb("init", HelpText = "Initialize support for a new game.")]
    public class Options {
        [Value(0, MetaName = "Id", Required = false, HelpText = "The game's ID (internal name).")]
        public string? Id { get; set; } = null;
    }

    public override async Task Execute(Options options) {
        string? pkg = null;
        SteamGameConfig? config = null;

        var unityVersions = await Http.GetManifest("UnityEngine.Modules", DepSource.BaGet);
        var id = options.Id ?? Prompt.Input<string>("What is the game's internal name? (Directory Name)");
        var name = Prompt.Input<string>("What is the game's name?");
        var pub = Prompt.Input<string>("Who published the game?");
        var src = Prompt.Select("Where is this package?", ["NuGet", "BaGet", "Steam"]);

        if (src == "Steam") {
            var appId = Prompt.Input<uint>("What is the game's Steam app ID?");
            var depotId = Prompt.Input<uint>("What is the game's Steam depot ID?");
            var folder = Prompt.Input<string>("Where is the folder for the game's assemblies?");
            var branch = Prompt.Input<string>("What is the branch to download from? (leave empty for default)", "");
            var os = Prompt.Input<string>("What OS should the assemblies be downloaded for? (leave empty for any)", "");

            var arch = Prompt.Input<string>(
                "What architecture should the assemblies be downloaded for? (leave empty for any)",
                ""
            );

            var language = Prompt.Input<string>(
                "What language should the assemblies be downloaded for? (leave empty for any)",
                ""
            );

            config = new SteamGameConfig {
                AppId = appId,
                DepotId = depotId,
                AssemblyFolder = folder,
                Branch = branch.Length == 0 ? null : branch,
                Os = os.Length == 0 ? null : os,
                Arch = arch.Length == 0 ? null : arch,
                Language = language.Length == 0 ? null : language,
            };
        } else {
            pkg = Prompt.Input<string>("What is the package for the game's assemblies?");
        }

        var ns = Prompt.Input<string>("What namespace should non-namespaced classes be relocated to?");
        var unity = Prompt.Select("What Unity version was the game made with?", unityVersions.versions);
        var framework = Prompt.Input<string>("What .NET framework was the game built with?");
        var unityFramework = framework == "netstandard2.1" ? "netstandard2.0" : framework;
        var steam = Prompt.Input<string>("What is the game's Steam page?");
        var hasWebsite = Prompt.Confirm("Does the game have a website?", true);
        var website = hasWebsite ? Prompt.Input<string>("What is the game's website?") : null;
        var hasThunderstore = Prompt.Confirm("Does the game have a Thunderstore community?", true);
        var thunderstore = hasThunderstore ? Prompt.Input<string>("What is the game's Thunderstore page?") : null;

        var it = new ProjectConfig {
            Game = new GameConfig {
                Id = id,
                Name = name,
                Package = pkg,
                Publisher = pub,
                Banner = "",
                Icon = "",
                Namespace = ns,
                Source = src.Parse(),
                Unity = unity,
                Steam = config,
            },

            Framework = new FrameworkConfig {
                Package = framework,
                Unity = unityFramework,
            },

            Urls = new UrlConfig {
                Steam = steam,
                Thunderstore = thunderstore,
                Website = website,
            },
        };

        it.Dependencies.Add(
            new Dependency {
                Name = "UnityEngine.Modules",
                Version = unity,
                Source = DepSource.BaGet,
            }
        );

        var dir = Path.Join("Games", id);

        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }

        var path = Path.Join(dir, "config.json");

        await File.WriteAllTextAsync(path, it.ToJsonString(Formatting.Indented));
    }
}
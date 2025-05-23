using System.Threading.Tasks;
using CommandLine;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Commands;

public sealed class Test : Command<Test.Options> {
    [Verb("test", HelpText = "Test new features")]
    public class Options;

    public override async Task Execute(Options options) {
        await SteamDownloader.DownloadAssemblies(
            new SteamGameConfig {
                AppId = 544550,
                DepotId = 544551,
                AssemblyFolder = "rocketstation_Data/Managed",
            }
        );
    }
}
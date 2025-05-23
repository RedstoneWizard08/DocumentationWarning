using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DepotDownloader;
using DocumentationWarning.Config;
using ShellProgressBar;

namespace DocumentationWarning.Util;

public class SteamDownloader : WithLogger {
    public static async Task<string> DownloadAssemblies(SteamGameConfig config) {
        AccountSettingsStore.LoadFromFile("account.config");

        ContentDownloader.Config.Silent = true;
        ContentDownloader.Config.MaxDownloads = 8;
        ContentDownloader.Config.UsingFileList = true;
        ContentDownloader.Config.FilesToDownload = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        ContentDownloader.Config.FilesToDownloadRegex = [
            new Regex(
                $@"{config.AssemblyFolder.Replace("/", "\\/")}\/.*",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            )
        ];

        var inst = new SteamDownloader();

        if (await InitializeSteam()) {
            "Logged in to Steam!".LogInfo(inst);

            var opts = new ProgressBarOptions {
                ForegroundColor = ConsoleColor.Cyan,
                DenseProgressBar = false,
                ProgressCharacter = '#',
                ShowEstimatedDuration = false,
            };

            var depotInfo = await ContentDownloader.GetDepotInfo(
                config.DepotId,
                config.AppId,
                ContentDownloader.INVALID_MANIFEST_ID,
                config.Branch ?? ContentDownloader.DEFAULT_BRANCH
            );

            var targetDir = Path.Combine(depotInfo.InstallDir, config.AssemblyFolder);

            $"App ID: {config.AppId}".LogInfo(inst);
            $"Depot ID: {config.DepotId}".LogInfo(inst);
            $"Manifest ID: {depotInfo.ManifestId}".LogInfo(inst);
            $"Install directory: {depotInfo.InstallDir}".LogInfo(inst);
            $"Target directory: {targetDir}".LogInfo(inst);

            try {
                using (var bar = new ProgressBar(100, "Downloading files...", opts)) {
                    var prog = bar.AsProgress<float>();

                    void callback(object sender, DownloadConfig.ProgressEventInfo info) {
                        bar.MaxTicks = (int) (info.bytesToDownload / 1024 / 1024);
                        prog.Report(info.percentage);
                    }

                    ContentDownloader.Config.ProgressCallback += callback;

                    await ContentDownloader.DownloadAppAsync(
                        config.AppId,
                        [(config.DepotId, ContentDownloader.INVALID_MANIFEST_ID)],
                        config.Branch ?? ContentDownloader.DEFAULT_BRANCH,
                        config.Os,
                        config.Arch,
                        config.Language,
                        false,
                        false
                    ).ConfigureAwait(false);

                    ContentDownloader.Config.ProgressCallback -= callback;
                }

                "Stopping Steam3 client...".LogInfo(new SteamDownloader());

                ShutdownSteam();

                "Done!".LogInfo(new SteamDownloader());
            } catch (Exception ex) when (ex is ContentDownloaderException or OperationCanceledException) {
                ex.Message.LogError(new SteamDownloader());
                Environment.Exit(1);
            } catch (Exception e) {
                $"Download failed to due to an unhandled exception: {e.Message}".LogError(new SteamDownloader());
                throw;
            } finally {
                ContentDownloader.ShutdownSteam3();
            }

            return targetDir;
        }

        "Failed to initialize Steam!".LogCritical(new SteamDownloader());
        Environment.Exit(1);
        return null;
    }

    private static async Task<bool> InitializeSteam() {
        var config = await ConfigHelper.GetSteamConfig();
        var inst = new SteamDownloader();

        "Logging in to Steam...".LogInfo(inst);

        return ContentDownloader.InitializeSteam3WithToken(config.FinalUsername, config.FinalLoginToken);
    }

    private static void ShutdownSteam() {
        ContentDownloader.ShutdownSteam3();
    }
}
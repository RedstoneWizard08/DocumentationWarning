using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Models;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public sealed class ExtractAsm : Step {
    protected override async Task Run(ProjectConfig config) {
        foreach (var dep in config.Dependencies) {
            var fileName = $"{dep.Name}-{dep.Version}.nupkg";
            var file = Path.Join(config.PkgDir, fileName);

            "Extracting assemblies from {}...".LogInfo(this, fileName);

            var zip = ZipFile.Open(file, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries) {
                if (!entry.Name.ToLower().EndsWith(".dll")) continue;

                var name = Path.GetFileName(entry.Name);
                var path = Path.Join(config.AsmDir, name);
                var stream = entry.Open();
                var mem = new MemoryStream();

                await stream.CopyToAsync(mem);

                var bytes = mem.ToArray();
                var handle = File.Create(path);

                await handle.WriteAsync(bytes);
                await handle.FlushAsync();

                handle.Close();
                await handle.DisposeAsync();
            }

            zip.Dispose();
        }

        if (config is { Game: { Source: DepSource.Steam, Steam: not null }, DownloadOutputDir: not null }) {
            "Extracting assemblies from Steam...".LogInfo(this, config.Game.Name);

            foreach (var entry in Directory.GetFiles(config.DownloadOutputDir)) {
                if (!entry.ToLower().EndsWith(".dll")) continue;

                var name = Path.GetFileName(entry);
                var path = Path.Join(config.AsmDir, name);

                File.Copy(entry, path, true);
            }
        }
    }
}
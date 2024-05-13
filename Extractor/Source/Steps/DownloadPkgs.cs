using System.IO;
using System.Threading.Tasks;
using Extractor.Config;

namespace Extractor.Steps;

public sealed class DownloadPkgs : Step
{
    public override async Task Run(ProjectConfig config)
    {
        foreach (var dep in config.Dependencies)
        {
            var url = dep.Source.GetPackageUrl(dep.Name, dep.Version);
            var fileName = $"{dep.Name}-{dep.Version}.nupkg";

            "Downloading {}...".LogInfo(this, fileName);

            var file = Path.Join(config.PkgDir, fileName);
            var resp = await Http.Get(url);
            var bytes = await resp.ReadAsByteArrayAsync();
            var handle = File.Create(file);

            await handle.WriteAsync(bytes);
            await handle.FlushAsync();

            handle.Close();
            await handle.DisposeAsync();
        }
    }
}

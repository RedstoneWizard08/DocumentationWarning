using System.IO;
using System.Threading.Tasks;

namespace Extractor.Steps;

public sealed class DownloadPkgs : Step
{
    public override async Task Run()
    {
        foreach (var dep in Config.Deps)
        {
            var url = dep.Item3.GetPackageUrl(dep.Item1, dep.Item2);
            var fileName = $"{dep.Item1}-{dep.Item2}.nupkg";

            "Downloading {}...".LogInfo(this, fileName);

            var file = Path.Join(Config.PkgDir, fileName);
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

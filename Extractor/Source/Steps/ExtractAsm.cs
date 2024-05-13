using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Extractor.Steps;

public sealed class ExtractAsm : Step
{
    public override async Task Run()
    {
        foreach (var dep in Config.Deps)
        {
            var fileName = $"{dep.Item1}-{dep.Item2}.nupkg";
            var file = Path.Join(Config.PkgDir, fileName);

            "Extracting assemblies from {}...".LogInfo(this, fileName);

            var zip = ZipFile.Open(file, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries)
            {
                if (entry.Name.ToLower().EndsWith(".dll"))
                {
                    var name = Path.GetFileName(entry.Name);
                    var path = Path.Join(Config.AsmDir, name);
                    var stream = entry.Open();
                    var mem = new MemoryStream();

                    await stream.CopyToAsync(mem);

                    var bytes = mem.ToArray();
                    var handle = File.Create(path);

                    await handle.WriteAsync(bytes);
                    await handle.FlushAsync();

                    handle.Close();
                    await handle.DisposeAsync();

                    State.Assemblies.Add(path);
                }
            }

            zip.Dispose();
        }
    }
}

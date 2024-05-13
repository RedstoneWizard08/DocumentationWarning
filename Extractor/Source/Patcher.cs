using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiffPatch;

namespace Extractor;

public sealed class Patcher: WithLogger
{
    public async Task Run(string file)
    {
        file = Path.Join("Patches", file);
        
        var data = await File.ReadAllTextAsync(file);
        var files = DiffParserHelper.Parse(data, Environment.NewLine).ToArray();

        foreach (var item in files)
        {
            if (!File.Exists(item.To))
            {
                "Cannot find file to patch: {}; Treating as an addition...".LogWarn(this, item.To);

                var content = "";

                foreach (var chunk in item.Chunks)
                {
                    foreach (var change in chunk.Changes)
                    {
                        if (change.Add)
                        {
                            content += change.Content + "\n";
                        }
                    }
                }

                if (!Directory.Exists(Path.GetDirectoryName(item.To)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(item.To)!);
                }

                await File.WriteAllTextAsync(item.To, content);

                continue;
            }

            var pre = await File.ReadAllTextAsync(item.To);
            var post = PatchHelper.Patch(pre, item.Chunks, Environment.NewLine).Trim();

            await File.WriteAllTextAsync(item.To, post);
        }
    }
}

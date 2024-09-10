using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiffPatch;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Processors;

public sealed class Patcher : WithLogger
{
    private readonly PatchHelper Helper = new();

    public async Task Run(ProjectConfig config, string path)
    {
        var data = await File.ReadAllTextAsync(path);
        var files = DiffParserHelper.Parse(data, Environment.NewLine).ToArray();

        foreach (var item in files)
        {
            var to = Path.Join(config.ProjectDir, item.To);

            if (!File.Exists(to))
            {
                "Cannot find file to patch: {}; Treating as an addition...".LogWarn(this, to);

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

                if (!Directory.Exists(Path.GetDirectoryName(to)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(to)!);
                }

                await File.WriteAllTextAsync(to, content);

                continue;
            }

            var pre = await File.ReadAllTextAsync(to);
            var post = Helper.Patch(pre, item.Chunks, Environment.NewLine).Trim();

            await File.WriteAllTextAsync(to, post);
        }
    }
}

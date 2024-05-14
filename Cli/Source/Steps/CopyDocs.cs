using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docfx.Common;
using DocumentationWarning.Config;
using DocumentationWarning.Models;
using Newtonsoft.Json;

namespace DocumentationWarning.Steps;

public sealed class CopyDocs : Step
{
    public const string OutputDir = "docs";
    public const string Manifest = "games.json";

    public override async Task Run(ProjectConfig config)
    {
        if (!Directory.Exists(OutputDir))
        {
            Directory.CreateDirectory(OutputDir);
        }

        var output = Path.Join(OutputDir, config.Game.Id);

        CopyDirectory(Path.Join(config.DocDir, "_site"), output, true);

        var data = new List<DocItem>();

        if (File.Exists(Manifest)) {
            var file = await File.ReadAllTextAsync(Manifest);
        
            data = JsonConvert.DeserializeObject<List<DocItem>>(file)!;
        }

        if (data.Find(v => v.Id == config.Game.Id) != null) return;

        data.Add(new DocItem()
        {
            Id = config.Game.Id,
            Name = config.Game.Name,
        });

        await File.WriteAllTextAsync(Manifest, data.ToJsonString());
    }


    private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        var dirs = dir.GetDirectories();

        Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}

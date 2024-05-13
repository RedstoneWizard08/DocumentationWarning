using Docfx.Dotnet;
using Docfx.Common;
using System.Threading.Tasks;
using Docfx;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Extractor.Config;

namespace Extractor.Steps;

public sealed class BuildDocs : Step
{
    public string? FindInPath(string filename)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path == null) return null;
        var directories = path.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ":" : ";");

        foreach (var dir in directories)
        {
            var fullpath = Path.Combine(dir, filename);
            if (File.Exists(fullpath)) return fullpath;
        }

        return null;
    }

    public override async Task Run(ProjectConfig config)
    {
        new ResExtractor().Extract(config.DocDir, config);

        if (!Directory.Exists(config.DocDir)) {
            Directory.CreateDirectory(config.DocDir);
        }

        File.Copy(config.Game.BannerPath(config), Path.Join(config.DocDir, config.Game.Banner));
        File.Copy(config.Game.IconPath(config), Path.Join(config.DocDir, config.Game.Icon));

        var start = new ProcessStartInfo()
        {
            FileName = FindInPath("docfx"),
            WorkingDirectory = config.DocDir,
        };

        "Starting process \"{}{}\" in {}...".LogInfo(this, start.FileName, start.Arguments != "" ? " " + start.Arguments : "", config.DocDir);

        var proc = new Process()
        {
            StartInfo = start,
        };

        proc.Start();
        await proc.WaitForExitAsync();
    }
}

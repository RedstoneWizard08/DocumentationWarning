using Docfx.Dotnet;
using Docfx.Common;
using System.Threading.Tasks;
using Docfx;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

    public override async Task Run()
    {
        var start = new ProcessStartInfo()
        {
            FileName = FindInPath("docfx"),
            WorkingDirectory = Path.Join(Environment.CurrentDirectory, ".."),
        };

        "Starting process \"{}{}\"...".LogInfo(this, start.FileName, start.Arguments != "" ? " " + start.Arguments : "");

        var proc = new Process()
        {
            StartInfo = start,
        };

        proc.Start();
        await proc.WaitForExitAsync();
    }
}

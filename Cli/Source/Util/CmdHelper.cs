using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DocumentationWarning.Util;

public sealed class CmdHelper: WithLogger {
    public static string? FindInPath(string filename)
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

    public static async Task Run(string dir, string command, params string[] args) {
        if (!command.StartsWith('/')) {
            command = FindInPath(command)!;
        }

        var start = new ProcessStartInfo()
        {
            FileName = command,
            WorkingDirectory = dir,
        };

        foreach (var arg in args) {
            start.ArgumentList.Add(arg);
        }
        
        var argsStr = args.Length > 0 ? " " + string.Join(" ", args) : "";

        "Starting process \"{}{}\" in {}...".LogInfo(new CmdHelper(), start.FileName, argsStr, dir);

        var proc = new Process()
        {
            StartInfo = start,
        };

        proc.Start();
        await proc.WaitForExitAsync();
    }
}

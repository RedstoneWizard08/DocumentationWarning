using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DocumentationWarning.Util;

public sealed class CmdHelper : WithLogger {
    public static string? FindInPath(string filename) {
        var path = Environment.GetEnvironmentVariable("PATH");

        return path?.Split(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ";"
                : ":"
        ).Select(
            item => Path.Combine(
                item,
                filename + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? ".exe"
                    : "")
            )
        ).FirstOrDefault(File.Exists);
    }

    public static async Task<string> Run(string dir, string cmd, bool output = false, params string[] args) {
        var command = cmd;

        if (!command.StartsWith('/')) {
            command = FindInPath(command)!;
        }

        var start = new ProcessStartInfo {
            FileName = command,
            WorkingDirectory = dir,
            RedirectStandardOutput = !output
        };

        foreach (var arg in args) {
            start.ArgumentList.Add(arg);
        }

        var argsStr = args.Length > 0 ? " " + string.Join(" ", args) : "";

        "Starting process \"{}{}\" in {}...".LogInfo(new CmdHelper(), start.FileName, argsStr, dir);

        var proc = new Process {
            StartInfo = start,
        };

        proc.Start();

        var data = "";

        if (!output) data = await proc.StandardOutput.ReadToEndAsync();

        await proc.WaitForExitAsync();

        return data;
    }
}
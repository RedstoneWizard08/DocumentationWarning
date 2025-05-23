using System;
using System.Linq;
using DocumentationWarning.Util;
using Pastel;

namespace DocumentationWarning.Logging;

public sealed class Logger(Type type) {
    private readonly Type type = type;

    private static string Format(string format, params object?[] args) {
        return args.Aggregate(format, (current, arg) => current.ReplaceFirst("{}", arg?.ToString()!));
    }

    public void Log(string prefix, ConsoleColor color, string format, params object?[] args) {
        var str = Format(format, args);
        var ty = $"[{type.Name}] ".Pastel(ConsoleColor.Magenta);
        var pfx = prefix.Pastel(color).PastelBg("#3f3f3f") + ": ";

        Console.WriteLine(ty + pfx + str);
    }

    public void LogInfo(string format, params object?[] args) => Log("info", ConsoleColor.Green, format, args);
    public void LogWarn(string format, params object?[] args) => Log("warn", ConsoleColor.DarkYellow, format, args);
    public void LogError(string format, params object?[] args) => Log("error", ConsoleColor.Red, format, args);

    public void LogCritical(string format, params object?[] args) =>
        Log("critical", ConsoleColor.DarkRed, format, args);

    public void LogDebug(string format, params object?[] args) => Log("debug", ConsoleColor.Gray, format, args);
}
namespace DocumentationWarning.Util;

public static class LoggerExt
{
    public static void LogInfo(this string str, WithLogger step, params object?[] args) => step.Logger.LogInfo(str, args);
    public static void LogWarn(this string str, WithLogger step, params object?[] args) => step.Logger.LogWarn(str, args);
    public static void LogError(this string str, WithLogger step, params object?[] args) => step.Logger.LogError(str, args);
    public static void LogCritical(this string str, WithLogger step, params object?[] args) => step.Logger.LogCritical(str, args);
    public static void LogDebug(this string str, WithLogger step, params object?[] args) => step.Logger.LogDebug(str, args);
}

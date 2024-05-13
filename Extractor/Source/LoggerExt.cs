using Microsoft.Extensions.Logging;

namespace Extractor;

public static class LoggerExt
{
    public static void LogInfo(this ILogger logger, string? message, params object?[] args) => logger.LogInformation(message, args);
    public static void LogWarn(this ILogger logger, string? message, params object?[] args) => logger.LogWarning(message, args);

    public static void LogInfo(this string str, WithLogger step, params object?[] args) => step.Logger.LogInfo(str, args);
    public static void LogWarn(this string str, WithLogger step, params object?[] args) => step.Logger.LogWarn(str, args);
    public static void LogError(this string str, WithLogger step, params object?[] args) => step.Logger.LogError(str, args);
    public static void LogCritical(this string str, WithLogger step, params object?[] args) => step.Logger.LogCritical(str, args);
    public static void LogDebug(this string str, WithLogger step, params object?[] args) => step.Logger.LogDebug(str, args);
    public static void LogTrace(this string str, WithLogger step, params object?[] args) => step.Logger.LogTrace(str, args);
}

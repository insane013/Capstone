using Microsoft.Extensions.Logging;

namespace TodoListApp.Helpers;

/// <summary>
/// WebApi related Log delegates.
/// </summary>
public static class LoggingDelegates
{
    /// <summary>
    /// Logs Incorrect model error.
    /// </summary>
    public static readonly Action<ILogger, string, Exception?> LogIncorrectData =
    LoggerMessage.Define<string?>(LogLevel.Error, new EventId(1, "IncorrectData"), "Incorrect data passed as a model.\n{ModelDetails}");

    /// <summary>
    /// Logs Unexpected Error.
    /// </summary>
    public static readonly Action<ILogger, string, Exception?> LogError =
    LoggerMessage.Define<string>(LogLevel.Error, new EventId(2, "Error"), "{Error}");

    /// <summary>
    /// Logs Warn.
    /// </summary>
    public static readonly Action<ILogger, string, Exception?> LogWarn =
    LoggerMessage.Define<string>(LogLevel.Warning, new EventId(3, "Warning"), "{Warn}");

    /// <summary>
    /// Logs Info.
    /// </summary>
    public static readonly Action<ILogger, string, Exception?> LogInfo =
    LoggerMessage.Define<string>(LogLevel.Information, new EventId(4, "InfoMsg"), "{Msg}");
}

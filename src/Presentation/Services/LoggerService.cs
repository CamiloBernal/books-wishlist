// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace BooksWishlist.Presentation.Services;

public class LoggerService : ILoggerService
{
    private readonly ILogger _logger;

    public LoggerService(ILogger logger) => _logger = logger;

    public void LogDebug(EventId eventId, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Debug, eventId, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogDebug(EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Debug, eventId, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogDebug(Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Debug, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogDebug(string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Debug, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogTrace(EventId eventId, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Trace, eventId, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogTrace(EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Trace, eventId, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogTrace(Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Trace, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogTrace(string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Trace, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogInformation(EventId eventId, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Information, eventId, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogInformation(EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Information, eventId, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogInformation(Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Information, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogInformation(string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Information, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogWarning(EventId eventId, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Warning, eventId, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogWarning(EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Warning, eventId, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogWarning(Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Warning, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogWarning(string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Warning, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogError(EventId eventId, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Error, eventId, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogError(EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Error, eventId, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogError(Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Error, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogError(string? message,
        Exception exception,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.LogError(exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    public void LogCritical(EventId eventId, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Critical, eventId, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogCritical(EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Critical, eventId, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogCritical(Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Critical, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void LogCritical(string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(LogLevel.Critical, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void Log(LogLevel logLevel, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(logLevel, 0, null, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void Log(LogLevel logLevel, EventId eventId, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(logLevel, eventId, null, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }


    public void Log(LogLevel logLevel, Exception? exception, string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object?[] args)
    {
        _logger.Log(logLevel, 0, exception, message, args);
        Watch(message, callerName, filePath, lineNumber);
    }

    private static void Watch(string? message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        WatchLogger.Log(message, callerName, filePath, lineNumber);
}

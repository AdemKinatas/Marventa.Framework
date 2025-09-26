using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Marventa.Framework.Infrastructure.Logging;

/// <summary>
/// Extension methods for structured logging with performance optimization
/// </summary>
public static class StructuredLogging
{
    // Pre-defined log messages for better performance
    private static readonly Action<ILogger, string, Exception?> _serviceStarting =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1000, "ServiceStarting"),
            "Service {ServiceName} is starting");

    private static readonly Action<ILogger, string, Exception?> _serviceStopping =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1001, "ServiceStopping"),
            "Service {ServiceName} is stopping");

    private static readonly Action<ILogger, string, string, Exception?> _databaseQueryExecuting =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(2000, "DatabaseQueryExecuting"),
            "Executing query: {Query} on database: {Database}");

    private static readonly Action<ILogger, string, long, Exception?> _databaseQueryExecuted =
        LoggerMessage.Define<string, long>(
            LogLevel.Information,
            new EventId(2001, "DatabaseQueryExecuted"),
            "Query executed on database: {Database} in {ElapsedMs}ms");

    private static readonly Action<ILogger, string, string, Exception?> _cacheHit =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(3000, "CacheHit"),
            "Cache hit for key: {Key} in cache: {CacheName}");

    private static readonly Action<ILogger, string, string, Exception?> _cacheMiss =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(3001, "CacheMiss"),
            "Cache miss for key: {Key} in cache: {CacheName}");

    private static readonly Action<ILogger, string, string, string, Exception?> _eventPublished =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(4000, "EventPublished"),
            "Event {EventType} published to {Destination} with correlation ID: {CorrelationId}");

    private static readonly Action<ILogger, string, string, Exception?> _eventReceived =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(4001, "EventReceived"),
            "Event {EventType} received with correlation ID: {CorrelationId}");

    private static readonly Action<ILogger, string, int, Exception?> _performanceWarning =
        LoggerMessage.Define<string, int>(
            LogLevel.Warning,
            new EventId(5000, "PerformanceWarning"),
            "Operation {Operation} took {ElapsedMs}ms which exceeds threshold");

    // Extension methods
    public static void LogServiceStarting(this ILogger logger, string serviceName)
        => _serviceStarting(logger, serviceName, null);

    public static void LogServiceStopping(this ILogger logger, string serviceName)
        => _serviceStopping(logger, serviceName, null);

    public static void LogDatabaseQueryExecuting(this ILogger logger, string query, string database)
        => _databaseQueryExecuting(logger, query, database, null);

    public static void LogDatabaseQueryExecuted(this ILogger logger, string database, long elapsedMs)
        => _databaseQueryExecuted(logger, database, elapsedMs, null);

    public static void LogCacheHit(this ILogger logger, string key, string cacheName = "default")
        => _cacheHit(logger, key, cacheName, null);

    public static void LogCacheMiss(this ILogger logger, string key, string cacheName = "default")
        => _cacheMiss(logger, key, cacheName, null);

    public static void LogEventPublished(this ILogger logger, string eventType, string destination, string correlationId)
        => _eventPublished(logger, eventType, destination, correlationId, null);

    public static void LogEventReceived(this ILogger logger, string eventType, string correlationId)
        => _eventReceived(logger, eventType, correlationId, null);

    public static void LogPerformanceWarning(this ILogger logger, string operation, int elapsedMs)
        => _performanceWarning(logger, operation, elapsedMs, null);

    /// <summary>
    /// Logs method entry with parameters
    /// </summary>
    public static IDisposable? LogMethodEntry(
        this ILogger logger,
        object? parameters = null,
        [CallerMemberName] string methodName = "")
    {
        if (!logger.IsEnabled(LogLevel.Debug))
            return null;

        logger.LogDebug("Entering {MethodName} with parameters: {@Parameters}", methodName, parameters);

        return new MethodLogger(logger, methodName);
    }

    private class MethodLogger : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _methodName;
        private readonly Stopwatch _stopwatch;

        public MethodLogger(ILogger logger, string methodName)
        {
            _logger = logger;
            _methodName = methodName;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogDebug("Exiting {MethodName} after {ElapsedMs}ms", _methodName, _stopwatch.ElapsedMilliseconds);
        }
    }
}
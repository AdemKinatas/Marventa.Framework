using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Marventa.Framework.Infrastructure.Logging;

/// <summary>
/// Performance logging scope for measuring operation duration
/// </summary>
public class PerformanceScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;
    private readonly int _thresholdMs;
    private readonly Dictionary<string, object> _properties;

    public PerformanceScope(ILogger logger, string operationName, int thresholdMs = 1000)
    {
        _logger = logger;
        _operationName = operationName;
        _thresholdMs = thresholdMs;
        _stopwatch = Stopwatch.StartNew();
        _properties = new Dictionary<string, object>();
    }

    public PerformanceScope AddProperty(string key, object value)
    {
        _properties[key] = value;
        return this;
    }

    public void Dispose()
    {
        _stopwatch.Stop();

        var logLevel = _stopwatch.ElapsedMilliseconds > _thresholdMs
            ? LogLevel.Warning
            : LogLevel.Information;

        using (_logger.BeginScope(_properties))
        {
            _logger.Log(logLevel,
                "Operation {OperationName} completed in {ElapsedMs}ms",
                _operationName,
                _stopwatch.ElapsedMilliseconds);
        }
    }
}
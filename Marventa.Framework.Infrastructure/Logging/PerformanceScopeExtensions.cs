using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Logging;

/// <summary>
/// Extension method for creating performance scopes
/// </summary>
public static class PerformanceScopeExtensions
{
    public static PerformanceScope BeginPerformanceScope(
        this ILogger logger,
        string operationName,
        int thresholdMs = 1000)
    {
        return new PerformanceScope(logger, operationName, thresholdMs);
    }
}
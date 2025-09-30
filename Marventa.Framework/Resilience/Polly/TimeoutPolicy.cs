using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace Marventa.Framework.Resilience.Polly;

public static class TimeoutPolicy
{
    public static AsyncTimeoutPolicy CreateTimeoutPolicy(int timeoutInSeconds = 30, ILogger? logger = null)
    {
        return Policy
            .TimeoutAsync(
                TimeSpan.FromSeconds(timeoutInSeconds),
                TimeoutStrategy.Pessimistic,
                onTimeoutAsync: (context, timeSpan, task) =>
                {
                    logger?.LogWarning("Operation timed out after {TimeoutSeconds}s", timeSpan.TotalSeconds);
                    return Task.CompletedTask;
                });
    }

    public static AsyncTimeoutPolicy<TResult> CreateTimeoutPolicy<TResult>(int timeoutInSeconds = 30, ILogger? logger = null)
    {
        return Policy
            .TimeoutAsync<TResult>(
                TimeSpan.FromSeconds(timeoutInSeconds),
                TimeoutStrategy.Pessimistic,
                onTimeoutAsync: (context, timeSpan, task) =>
                {
                    logger?.LogWarning("Operation timed out after {TimeoutSeconds}s", timeSpan.TotalSeconds);
                    return Task.CompletedTask;
                });
    }
}

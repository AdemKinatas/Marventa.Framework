using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace Marventa.Framework.Resilience.Polly;

public static class CircuitBreakerPolicy
{
    public static AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy(
        int exceptionsAllowedBeforeBreaking = 3,
        int durationOfBreakInSeconds = 30,
        ILogger? logger = null)
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(durationOfBreakInSeconds),
                onBreak: (exception, duration) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Circuit breaker opened for {BreakDuration}s due to {ExceptionMessage}",
                        duration.TotalSeconds,
                        exception.Message);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Circuit breaker half-open");
                });
    }

    public static AsyncCircuitBreakerPolicy<TResult> CreateCircuitBreakerPolicy<TResult>(
        int exceptionsAllowedBeforeBreaking = 3,
        int durationOfBreakInSeconds = 30,
        ILogger? logger = null)
    {
        return Policy<TResult>
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(durationOfBreakInSeconds),
                onBreak: (outcome, duration) =>
                {
                    logger?.LogWarning(
                        outcome.Exception,
                        "Circuit breaker opened for {BreakDuration}s due to {ExceptionMessage}",
                        duration.TotalSeconds,
                        outcome.Exception?.Message);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Circuit breaker half-open");
                });
    }
}

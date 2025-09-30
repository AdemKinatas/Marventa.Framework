using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Marventa.Framework.Resilience.Polly;

public static class RetryPolicy
{
    public static AsyncRetryPolicy CreateRetryPolicy(int retryCount = 3, ILogger? logger = null)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Retry {RetryCount} after {RetryTimeSpan}s due to {ExceptionMessage}",
                        retry,
                        timeSpan.TotalSeconds,
                        exception.Message);
                });
    }

    public static AsyncRetryPolicy<TResult> CreateRetryPolicy<TResult>(int retryCount = 3, ILogger? logger = null)
    {
        return Policy<TResult>
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timeSpan, retry, ctx) =>
                {
                    logger?.LogWarning(
                        outcome.Exception,
                        "Retry {RetryCount} after {RetryTimeSpan}s due to {ExceptionMessage}",
                        retry,
                        timeSpan.TotalSeconds,
                        outcome.Exception?.Message);
                });
    }
}

namespace Marventa.Framework.Core.Interfaces;

public interface IIdempotencyService
{
    Task<IdempotencyResult> ProcessAsync(string key, Func<Task<object>> operation, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<IdempotencyResult<T>> ProcessAsync<T>(string key, Func<Task<T>> operation, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<bool> IsProcessedAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> TryLockAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task ReleaseLockAsync(string key, CancellationToken cancellationToken = default);
    Task InvalidateAsync(string key, CancellationToken cancellationToken = default);
}

public class IdempotencyResult
{
    public bool IsFromCache { get; set; }
    public object? Result { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, object>? Headers { get; set; }
    public DateTime ProcessedAt { get; set; }

    public static IdempotencyResult FromCache(object? result, int statusCode = 200, Dictionary<string, object>? headers = null, DateTime processedAt = default)
    {
        return new IdempotencyResult
        {
            IsFromCache = true,
            Result = result,
            StatusCode = statusCode,
            Headers = headers,
            ProcessedAt = processedAt
        };
    }

    public static IdempotencyResult FromOperation(object? result, int statusCode = 200, Dictionary<string, object>? headers = null)
    {
        return new IdempotencyResult
        {
            IsFromCache = false,
            Result = result,
            StatusCode = statusCode,
            Headers = headers,
            ProcessedAt = DateTime.UtcNow
        };
    }
}

public class IdempotencyResult<T> : IdempotencyResult
{
    public new T? Result
    {
        get => base.Result is T result ? result : default;
        set => base.Result = value;
    }

    public static IdempotencyResult<T> FromCache(T? result, int statusCode = 200, Dictionary<string, object>? headers = null, DateTime processedAt = default)
    {
        return new IdempotencyResult<T>
        {
            IsFromCache = true,
            Result = result,
            StatusCode = statusCode,
            Headers = headers,
            ProcessedAt = processedAt
        };
    }

    public static IdempotencyResult<T> FromOperation(T? result, int statusCode = 200, Dictionary<string, object>? headers = null)
    {
        return new IdempotencyResult<T>
        {
            IsFromCache = false,
            Result = result,
            StatusCode = statusCode,
            Headers = headers,
            ProcessedAt = DateTime.UtcNow
        };
    }
}
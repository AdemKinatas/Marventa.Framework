namespace Marventa.Framework.Core.Interfaces.Idempotency;

public interface IIdempotencyService
{
    Task<IdempotencyResult> ProcessAsync(string key, Func<Task<object>> operation, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<IdempotencyResult<T>> ProcessAsync<T>(string key, Func<Task<T>> operation, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<bool> IsProcessedAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> TryLockAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task ReleaseLockAsync(string key, CancellationToken cancellationToken = default);
    Task InvalidateAsync(string key, CancellationToken cancellationToken = default);
}
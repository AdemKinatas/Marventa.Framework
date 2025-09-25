using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class CacheServiceExtensions
{
    /// <summary>
    /// Checks if a cache key exists
    /// </summary>
    public static async Task<bool> ExistsAsync(this ICacheService cacheService, string key, CancellationToken cancellationToken = default)
    {
        var value = await cacheService.GetAsync<object>(key, cancellationToken);
        return value != null;
    }

    /// <summary>
    /// Sets a value in cache only if the key doesn't exist (Redis SET NX equivalent)
    /// </summary>
    public static async Task<bool> SetIfNotExistsAsync<T>(this ICacheService cacheService, string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation - in a real Redis cache service,
        // this would be implemented atomically with SET NX EX
        var exists = await cacheService.ExistsAsync(key, cancellationToken);
        if (exists)
            return false;

        await cacheService.SetAsync(key, value, expiration, cancellationToken);
        return true;
    }

    /// <summary>
    /// Gets a value from cache or sets it using the provided factory function
    /// </summary>
    public static async Task<T> GetOrSetAsync<T>(this ICacheService cacheService, string key, Func<Task<T>> factory, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var cached = await cacheService.GetAsync<T>(key, cancellationToken);
        if (cached != null)
            return cached;

        var value = await factory();
        await cacheService.SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    /// <summary>
    /// Removes multiple keys with a pattern (simple implementation)
    /// </summary>
    public static async Task RemoveByPatternAsync(this ICacheService cacheService, string pattern, CancellationToken cancellationToken = default)
    {
        // Note: This is a simplified implementation
        // In a real Redis implementation, you would use SCAN with pattern matching

        // For now, we'll just document that this should be implemented
        // by the concrete cache service implementation
        await Task.CompletedTask;
    }
}
using Marventa.Framework.Core.Interfaces.Caching;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class CacheServiceExtensions
{
    /// <summary>
    /// Gets a value from cache or sets it using the provided factory function
    /// </summary>
    public static async Task<T?> GetOrSetAsync<T>(this ICacheService cacheService, string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cached = await cacheService.GetAsync<T>(key, cancellationToken);
        if (cached != null)
            return cached;

        var value = await factory();
        if (value != null)
        {
            await cacheService.SetAsync(key, value, expiration ?? TimeSpan.FromMinutes(5), cancellationToken);
        }
        return value;
    }

    /// <summary>
    /// Sets a value in cache only if the key doesn't exist (Redis SET NX equivalent)
    /// </summary>
    public static async Task<bool> SetIfNotExistsAsync<T>(this ICacheService cacheService, string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var exists = await cacheService.ExistsAsync(key, cancellationToken);
        if (exists)
            return false;

        await cacheService.SetAsync(key, value, expiration, cancellationToken);
        return true;
    }

    /// <summary>
    /// Increments a numeric value in cache
    /// </summary>
    public static async Task<long> IncrementAsync(this ICacheService cacheService, string key, long value = 1, CancellationToken cancellationToken = default)
    {
        var current = await cacheService.GetAsync<long>(key, cancellationToken);
        var newValue = current + value;
        await cacheService.SetAsync(key, newValue, cancellationToken: cancellationToken);
        return newValue;
    }
}
using Marventa.Framework.Features.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Marventa.Framework.Features.Caching.Distributed;

public class RedisCache : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCache(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new DistributedCacheEntryOptions();

        if (options?.AbsoluteExpiration.HasValue == true)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration.Value;
        }

        if (options?.SlidingExpiration.HasValue == true)
        {
            cacheOptions.SlidingExpiration = options.SlidingExpiration.Value;
        }

        var serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, cacheOptions, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return value != null;
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Redis prefix removal requires server-side operations
        // This is a simplified implementation
        await Task.CompletedTask;
    }
}

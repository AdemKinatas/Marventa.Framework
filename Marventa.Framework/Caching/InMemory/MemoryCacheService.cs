using Marventa.Framework.Caching.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Marventa.Framework.Caching.InMemory;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _keys = new();

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new MemoryCacheEntryOptions();

        if (options?.AbsoluteExpiration.HasValue == true)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration.Value;
        }

        if (options?.SlidingExpiration.HasValue == true)
        {
            cacheOptions.SlidingExpiration = options.SlidingExpiration.Value;
        }

        _cache.Set(key, value, cacheOptions);
        _keys.Add(key);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _keys.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _keys.Where(k => k.StartsWith(prefix)).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _keys.Remove(key);
        }
        return Task.CompletedTask;
    }
}

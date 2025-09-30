using Marventa.Framework.Caching.Abstractions;

namespace Marventa.Framework.Caching.Hybrid;

public class HybridCacheService : ICacheService
{
    private readonly ICacheService _memoryCache;
    private readonly ICacheService _distributedCache;

    public HybridCacheService(ICacheService memoryCache, ICacheService distributedCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // Try memory cache first
        var value = await _memoryCache.GetAsync<T>(key, cancellationToken);
        if (value != null)
        {
            return value;
        }

        // If not in memory, try distributed cache
        value = await _distributedCache.GetAsync<T>(key, cancellationToken);
        if (value != null)
        {
            // Store in memory cache for faster access
            await _memoryCache.SetAsync(key, value, CacheOptions.WithSlidingExpiration(TimeSpan.FromMinutes(5)), cancellationToken);
        }

        return value;
    }

    public async Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Set in both caches
        await Task.WhenAll(
            _memoryCache.SetAsync(key, value, options, cancellationToken),
            _distributedCache.SetAsync(key, value, options, cancellationToken)
        );
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // Remove from both caches
        await Task.WhenAll(
            _memoryCache.RemoveAsync(key, cancellationToken),
            _distributedCache.RemoveAsync(key, cancellationToken)
        );
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        // Check memory cache first
        var existsInMemory = await _memoryCache.ExistsAsync(key, cancellationToken);
        if (existsInMemory)
        {
            return true;
        }

        // Check distributed cache
        return await _distributedCache.ExistsAsync(key, cancellationToken);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Remove from both caches
        await Task.WhenAll(
            _memoryCache.RemoveByPrefixAsync(prefix, cancellationToken),
            _distributedCache.RemoveByPrefixAsync(prefix, cancellationToken)
        );
    }
}

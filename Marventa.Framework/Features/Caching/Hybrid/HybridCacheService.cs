using Marventa.Framework.Features.Caching.Abstractions;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Features.Caching.Hybrid;

public class HybridCacheService : ICacheService
{
    private readonly ICacheService _memoryCache;
    private readonly ICacheService _distributedCache;
    private readonly ILogger<HybridCacheService>? _logger;

    public HybridCacheService(
        ICacheService memoryCache,
        ICacheService distributedCache,
        ILogger<HybridCacheService>? logger = null)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // Try memory cache first (L1)
        var value = await _memoryCache.GetAsync<T>(key, cancellationToken);
        if (value != null)
        {
            return value;
        }

        // If not in memory, try distributed cache (L2)
        try
        {
            value = await _distributedCache.GetAsync<T>(key, cancellationToken);
            if (value != null)
            {
                // Store in memory cache for faster access
                await _memoryCache.SetAsync(key, value, CacheOptions.WithSlidingExpiration(TimeSpan.FromMinutes(5)), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Distributed cache GET operation failed for key {Key}. Falling back to memory cache only.", key);
        }

        return value;
    }

    public async Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Always set in memory cache
        await _memoryCache.SetAsync(key, value, options, cancellationToken);

        // Try to set in distributed cache, but don't fail if it's unavailable
        try
        {
            await _distributedCache.SetAsync(key, value, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Distributed cache SET operation failed for key {Key}. Value is still cached in memory.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // Always remove from memory cache
        await _memoryCache.RemoveAsync(key, cancellationToken);

        // Try to remove from distributed cache
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Distributed cache REMOVE operation failed for key {Key}.", key);
        }
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
        try
        {
            return await _distributedCache.ExistsAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Distributed cache EXISTS operation failed for key {Key}. Falling back to memory cache result.", key);
            return false;
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Always remove from memory cache
        await _memoryCache.RemoveByPrefixAsync(prefix, cancellationToken);

        // Try to remove from distributed cache
        try
        {
            await _distributedCache.RemoveByPrefixAsync(prefix, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Distributed cache REMOVE BY PREFIX operation failed for prefix {Prefix}.", prefix);
        }
    }
}

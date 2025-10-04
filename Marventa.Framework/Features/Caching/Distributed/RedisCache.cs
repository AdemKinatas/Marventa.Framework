using Marventa.Framework.Features.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Marventa.Framework.Features.Caching.Distributed;

/// <summary>
/// Redis-based implementation of the cache service.
/// </summary>
public class RedisCache : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer? _connectionMultiplexer;

    /// <summary>
    /// Initializes a new instance of the RedisCache class.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="connectionMultiplexer">Optional Redis connection multiplexer for advanced operations.</param>
    public RedisCache(IDistributedCache cache, IConnectionMultiplexer? connectionMultiplexer = null)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _connectionMultiplexer = connectionMultiplexer;
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

    /// <summary>
    /// Removes all cache entries with keys starting with the specified prefix.
    /// Requires IConnectionMultiplexer to be injected for server-side operations.
    /// </summary>
    /// <param name="prefix">The key prefix to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty.", nameof(prefix));

        if (_connectionMultiplexer == null)
        {
            // If ConnectionMultiplexer is not available, log a warning or throw
            // For now, we'll silently return to maintain backward compatibility
            return;
        }

        try
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();

            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.GetServer(endpoint);

                // Skip replica servers to avoid errors
                if (server.IsReplica)
                    continue;

                // Get all keys matching the pattern
                var keys = server.Keys(pattern: $"{prefix}*", pageSize: 1000);

                if (keys == null)
                    continue;

                var database = _connectionMultiplexer.GetDatabase();
                var keysToDelete = keys.Select(k => (RedisKey)k.ToString()).ToArray();

                if (keysToDelete.Length > 0)
                {
                    await database.KeyDeleteAsync(keysToDelete);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception in production
            // For now, we'll rethrow to make the caller aware
            throw new InvalidOperationException($"Failed to remove keys with prefix '{prefix}'.", ex);
        }
    }
}

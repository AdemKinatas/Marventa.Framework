using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Marventa.Framework.Features.Idempotency;

/// <summary>
/// Redis-based implementation of idempotency service.
/// Stores request/response pairs in Redis with a configurable TTL.
/// </summary>
public class IdempotencyService : IIdempotencyService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(24);
    private const string KeyPrefix = "idempotency:";

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyService"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    public IdempotencyService(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc/>
    public async Task<bool> IsProcessedAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Idempotency key cannot be null or empty.", nameof(key));

        var cacheKey = GetCacheKey(key);
        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);

        return !string.IsNullOrEmpty(cachedValue);
    }

    /// <inheritdoc/>
    public async Task<IdempotentResponse?> GetResponseAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Idempotency key cannot be null or empty.", nameof(key));

        var cacheKey = GetCacheKey(key);
        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(cachedValue))
            return null;

        try
        {
            return JsonSerializer.Deserialize<IdempotentResponse>(cachedValue);
        }
        catch (JsonException)
        {
            // If deserialization fails, treat as not found
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task SetResponseAsync(string key, IdempotentResponse response, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Idempotency key cannot be null or empty.", nameof(key));

        if (response == null)
            throw new ArgumentNullException(nameof(response));

        var cacheKey = GetCacheKey(key);
        var serializedResponse = JsonSerializer.Serialize(response);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _defaultExpiration
        };

        await _cache.SetStringAsync(cacheKey, serializedResponse, options, cancellationToken);
    }

    /// <summary>
    /// Generates the full cache key with prefix.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <returns>The full cache key.</returns>
    private static string GetCacheKey(string key)
    {
        return $"{KeyPrefix}{key}";
    }
}

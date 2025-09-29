using System.Text.Json;
using Marventa.Framework.Core.Interfaces.Idempotency;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Marventa.Framework.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marventa.Framework.Infrastructure.Idempotency;

public class IdempotencyService : IIdempotencyService
{
    private readonly ICacheService _cache;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<IdempotencyService> _logger;
    private readonly IdempotencyOptions _options;

    public IdempotencyService(
        ICacheService cache,
        ITenantContext tenantContext,
        IOptions<IdempotencyOptions> options,
        ILogger<IdempotencyService> logger)
    {
        _cache = cache;
        _tenantContext = tenantContext;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IdempotencyResult> ProcessAsync(string key, Func<Task<object>> operation, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        return await ProcessAsync<object>(key, operation, expiration, cancellationToken);
    }

    public async Task<IdempotencyResult<T>> ProcessAsync<T>(string key, Func<Task<T>> operation, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(key);
        expiration ??= _options.DefaultExpiration;

        // Check if result already exists
        var cachedResult = await GetCachedResultAsync<T>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Idempotency key found in cache: {Key}", key);
            return cachedResult;
        }

        // Try to acquire lock
        var lockKey = BuildLockKey(key);
        var lockAcquired = await TryLockAsync(lockKey, TimeSpan.FromMinutes(5), cancellationToken);

        if (!lockAcquired)
        {
            // Wait a bit and check cache again (another request might have completed)
            await Task.Delay(100, cancellationToken);
            cachedResult = await GetCachedResultAsync<T>(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            throw new InvalidOperationException($"Could not acquire lock for idempotency key: {key}");
        }

        try
        {
            // Double-check cache after acquiring lock
            cachedResult = await GetCachedResultAsync<T>(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            // Execute operation
            _logger.LogDebug("Executing operation for idempotency key: {Key}", key);
            var result = await operation();

            // Cache the result
            var idempotencyResult = IdempotencyResult<T>.FromOperation(result);
            await CacheResultAsync(cacheKey, idempotencyResult, expiration.Value, cancellationToken);

            _logger.LogDebug("Operation completed and cached for idempotency key: {Key}", key);
            return idempotencyResult;
        }
        finally
        {
            await ReleaseLockAsync(lockKey, cancellationToken);
        }
    }

    public async Task<bool> IsProcessedAsync(string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(key);
        return await _cache.ExistsAsync(cacheKey, cancellationToken);
    }

    public async Task<bool> TryLockAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var lockKey = BuildLockKey(key);
        var lockValue = Guid.NewGuid().ToString();

        try
        {
            // Try to set the lock (Redis SET NX EX equivalent)
            var acquired = await _cache.SetIfNotExistsAsync(lockKey, lockValue, expiration, cancellationToken);
            if (acquired)
            {
                _logger.LogDebug("Lock acquired: {LockKey}", lockKey);
            }
            return acquired;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire lock: {LockKey}", lockKey);
            return false;
        }
    }

    public async Task ReleaseLockAsync(string key, CancellationToken cancellationToken = default)
    {
        var lockKey = BuildLockKey(key);
        try
        {
            await _cache.RemoveAsync(lockKey, cancellationToken);
            _logger.LogDebug("Lock released: {LockKey}", lockKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to release lock: {LockKey}", lockKey);
        }
    }

    public async Task InvalidateAsync(string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(key);
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        _logger.LogDebug("Idempotency key invalidated: {Key}", key);
    }

    private string BuildCacheKey(string key)
    {
        var tenantId = _tenantContext.TenantId ?? "default";
        return $"{_options.KeyPrefix}:result:{tenantId}:{key}";
    }

    private string BuildLockKey(string key)
    {
        var tenantId = _tenantContext.TenantId ?? "default";
        return $"{_options.KeyPrefix}:lock:{tenantId}:{key}";
    }

    private async Task<IdempotencyResult<T>?> GetCachedResultAsync<T>(string cacheKey, CancellationToken cancellationToken)
    {
        try
        {
            var cached = await _cache.GetAsync<string>(cacheKey, cancellationToken);
            if (string.IsNullOrEmpty(cached))
                return null;

            var result = JsonSerializer.Deserialize<IdempotencyResult<T>>(cached);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize cached idempotency result: {CacheKey}", cacheKey);
            return null;
        }
    }

    private async Task CacheResultAsync<T>(string cacheKey, IdempotencyResult<T> result, TimeSpan expiration, CancellationToken cancellationToken)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(result);
            await _cache.SetAsync(cacheKey, serialized, expiration, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache idempotency result: {CacheKey}", cacheKey);
        }
    }
}

public class IdempotencyOptions
{
    public const string SectionName = "Idempotency";

    public string KeyPrefix { get; set; } = "idempotency";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(24);
    public string HeaderName { get; set; } = "X-Idempotency-Key";
    public bool RequireIdempotencyKey { get; set; } = false;
    public List<string> IdempotentMethods { get; set; } = new() { "POST", "PUT", "PATCH" };
    public List<string> IgnoredPaths { get; set; } = new() { "/health", "/metrics" };
}
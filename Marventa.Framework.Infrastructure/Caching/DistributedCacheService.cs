using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Core.Interfaces.MultiTenancy;

namespace Marventa.Framework.Infrastructure.Caching;

/// <summary>
/// Wrapper for Microsoft.Extensions.Caching.Distributed
/// Allows using either Redis or SQL Server distributed cache
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DistributedCacheService(
        IDistributedCache distributedCache,
        ITenantContext tenantContext,
        ILogger<DistributedCacheService> logger)
    {
        _distributedCache = distributedCache;
        _tenantContext = tenantContext;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            var bytes = await _distributedCache.GetAsync(fullKey, cancellationToken);

            if (bytes == null || bytes.Length == 0)
            {
                _logger.LogDebug("Cache miss for key: {Key}", fullKey);
                return default;
            }

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            _logger.LogDebug("Cache hit for key: {Key}", fullKey);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
                options.SlidingExpiration = expiration.Value;
            else
                options.SlidingExpiration = TimeSpan.FromMinutes(5);

            await _distributedCache.SetAsync(fullKey, bytes, options, cancellationToken);
            _logger.LogDebug("Cache set for key: {Key}", fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            await _distributedCache.RemoveAsync(fullKey, cancellationToken);
            _logger.LogDebug("Cache removed for key: {Key}", fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("RemoveByPattern is not supported by IDistributedCache. Use RedisCacheService for this feature.");
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<object>(key, cancellationToken);
        return value != null;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Clear is not supported by IDistributedCache. Use RedisCacheService for this feature.");
        return Task.CompletedTask;
    }

    private string BuildKey(string key)
    {
        if (_tenantContext.CurrentTenant != null)
            return $"tenant:{_tenantContext.CurrentTenant.Id}:{key}";

        return key;
    }
}
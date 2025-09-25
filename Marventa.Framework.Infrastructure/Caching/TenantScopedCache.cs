using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Marventa.Framework.Core.Interfaces;
using System.Text.Json;

namespace Marventa.Framework.Infrastructure.Caching;

public class TenantScopedCache : ITenantScopedCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly ITenantContext _tenantContext;
    private readonly TenantCacheOptions _options;

    public TenantScopedCache(
        IDistributedCache distributedCache,
        ITenantContext tenantContext,
        IOptions<TenantCacheOptions> options)
    {
        _distributedCache = distributedCache;
        _tenantContext = tenantContext;
        _options = options.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantScopedKey(key);
        var cached = await _distributedCache.GetStringAsync(tenantKey, cancellationToken);

        if (string.IsNullOrEmpty(cached))
            return default;

        return JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantScopedKey(key);
        var json = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration
        };

        await _distributedCache.SetStringAsync(tenantKey, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantScopedKey(key);
        await _distributedCache.RemoveAsync(tenantKey, cancellationToken);
    }

    public async Task ClearTenantCacheAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        // This is a simplified approach - in production you'd want to track keys per tenant
        var pattern = $"tenant:{tenantId}:*";
        // Implementation depends on the cache provider (Redis, SQL Server, etc.)
        await Task.CompletedTask;
    }

    private string GetTenantScopedKey(string key)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        return $"tenant:{tenantId}:{key}";
    }
}
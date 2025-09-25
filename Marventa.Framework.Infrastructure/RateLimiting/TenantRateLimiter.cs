using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.RateLimiting;

public interface ITenantRateLimiter
{
    Task<bool> IsAllowedAsync(string resource, int cost = 1, CancellationToken cancellationToken = default);
    Task<RateLimitResult> CheckLimitAsync(string resource, CancellationToken cancellationToken = default);
    Task ResetAsync(string resource, CancellationToken cancellationToken = default);
}

public class TenantRateLimiter : ITenantRateLimiter
{
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;
    private readonly TenantRateLimitOptions _options;

    public TenantRateLimiter(
        IDistributedCache cache,
        ITenantContext tenantContext,
        IOptions<TenantRateLimitOptions> options)
    {
        _cache = cache;
        _tenantContext = tenantContext;
        _options = options.Value;
    }

    public async Task<bool> IsAllowedAsync(string resource, int cost = 1, CancellationToken cancellationToken = default)
    {
        var result = await CheckLimitAsync(resource, cancellationToken);

        if (result.RemainingRequests < cost)
            return false;

        await IncrementCounterAsync(resource, cost, cancellationToken);
        return true;
    }

    public async Task<RateLimitResult> CheckLimitAsync(string resource, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        var key = GetRateLimitKey(tenantId, resource);

        var countStr = await _cache.GetStringAsync(key, cancellationToken);
        var currentCount = string.IsNullOrEmpty(countStr) ? 0 : int.Parse(countStr);

        var limit = GetTenantLimit(tenantId, resource);

        return new RateLimitResult
        {
            Limit = limit,
            CurrentCount = currentCount,
            RemainingRequests = Math.Max(0, limit - currentCount),
            ResetsAt = DateTime.UtcNow.Add(_options.Window)
        };
    }

    public async Task ResetAsync(string resource, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        var key = GetRateLimitKey(tenantId, resource);
        await _cache.RemoveAsync(key, cancellationToken);
    }

    private async Task IncrementCounterAsync(string resource, int cost, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        var key = GetRateLimitKey(tenantId, resource);

        var countStr = await _cache.GetStringAsync(key, cancellationToken);
        var currentCount = string.IsNullOrEmpty(countStr) ? 0 : int.Parse(countStr);

        var newCount = currentCount + cost;

        await _cache.SetStringAsync(
            key,
            newCount.ToString(),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = _options.Window
            },
            cancellationToken);
    }

    private string GetRateLimitKey(string tenantId, string resource)
    {
        return $"ratelimit:{tenantId}:{resource}:{DateTime.UtcNow:yyyyMMddHHmm}";
    }

    private int GetTenantLimit(string tenantId, string resource)
    {
        // You can have different limits per tenant tier
        if (_options.TenantLimits.TryGetValue(tenantId, out var tenantLimits))
        {
            if (tenantLimits.TryGetValue(resource, out var limit))
                return limit;
        }

        return _options.DefaultLimit;
    }
}

public class RateLimitResult
{
    public int Limit { get; set; }
    public int CurrentCount { get; set; }
    public int RemainingRequests { get; set; }
    public DateTime ResetsAt { get; set; }
}

public class TenantRateLimitOptions
{
    public const string SectionName = "TenantRateLimit";
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
    public int DefaultLimit { get; set; } = 100;
    public Dictionary<string, Dictionary<string, int>> TenantLimits { get; set; } = new();
}
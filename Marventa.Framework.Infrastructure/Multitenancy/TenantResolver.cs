using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marventa.Framework.Infrastructure.Multitenancy;

public class TenantResolver : ITenantResolver<HttpContext>
{
    private readonly ITenantStore _tenantStore;
    private readonly ILogger<TenantResolver> _logger;
    private readonly TenantOptions _options;

    public TenantResolver(
        ITenantStore tenantStore,
        IOptions<TenantOptions> options,
        ILogger<TenantResolver> logger)
    {
        _tenantStore = tenantStore;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ITenant?> ResolveAsync(HttpContext context)
    {
        try
        {
            // Strategy 1: Header-based resolution
            if (_options.ResolutionStrategies.Contains(TenantResolutionStrategy.Header))
            {
                var tenantFromHeader = await ResolveFromHeaderAsync(context);
                if (tenantFromHeader != null)
                {
                    _logger.LogDebug("Tenant resolved from header: {TenantId}", tenantFromHeader.Id);
                    return tenantFromHeader;
                }
            }

            // Strategy 2: Host-based resolution
            if (_options.ResolutionStrategies.Contains(TenantResolutionStrategy.Host))
            {
                var tenantFromHost = await ResolveFromHostAsync(context);
                if (tenantFromHost != null)
                {
                    _logger.LogDebug("Tenant resolved from host: {TenantId}", tenantFromHost.Id);
                    return tenantFromHost;
                }
            }

            // Strategy 3: Path-based resolution
            if (_options.ResolutionStrategies.Contains(TenantResolutionStrategy.Path))
            {
                var tenantFromPath = await ResolveFromPathAsync(context);
                if (tenantFromPath != null)
                {
                    _logger.LogDebug("Tenant resolved from path: {TenantId}", tenantFromPath.Id);
                    return tenantFromPath;
                }
            }

            _logger.LogWarning("No tenant could be resolved for request: {Path}", context.Request.Path);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant for request: {Path}", context.Request.Path);
            return null;
        }
    }

    private async Task<ITenant?> ResolveFromHeaderAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(_options.TenantHeaderName, out var tenantId))
            return null;

        var tenant = await _tenantStore.GetByIdAsync(tenantId.ToString());
        return tenant;
    }

    private async Task<ITenant?> ResolveFromHostAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;
        return await _tenantStore.GetByHostAsync(host);
    }

    private async Task<ITenant?> ResolveFromPathAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path))
            return null;

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return null;

        // Assume first segment is tenant identifier
        var tenantIdentifier = segments[0];
        return await _tenantStore.GetByIdentifierAsync(tenantIdentifier);
    }
}

public class TenantOptions
{
    public const string SectionName = "Multitenancy";

    public List<TenantResolutionStrategy> ResolutionStrategies { get; set; } =
        new() { TenantResolutionStrategy.Header };

    public string TenantHeaderName { get; set; } = "X-Tenant-Id";
    public bool RequireTenant { get; set; } = true;
    public bool EnableTenantCaching { get; set; } = true;
    public TimeSpan TenantCacheDuration { get; set; } = TimeSpan.FromMinutes(30);
    public string? DefaultTenantId { get; set; }
}

public enum TenantResolutionStrategy
{
    Header,
    Host,
    Path,
    Claim
}
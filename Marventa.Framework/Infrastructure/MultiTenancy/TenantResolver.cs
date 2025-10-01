using Microsoft.AspNetCore.Http;

namespace Marventa.Framework.Infrastructure.MultiTenancy;

public class TenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? ResolveTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        // Try to get tenant from header
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader, out var tenantId))
            {
                return tenantId;
            }
        }

        // Try to get tenant from claims
        var tenantClaim = httpContext.User.FindFirst("tenant_id");
        if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantIdFromClaim))
        {
            return tenantIdFromClaim;
        }

        return null;
    }

    public string? ResolveTenantName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        // Try to get tenant name from header
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Name", out var tenantNameHeader))
        {
            return tenantNameHeader.ToString();
        }

        // Try to get tenant name from claims
        var tenantClaim = httpContext.User.FindFirst("tenant_name");
        return tenantClaim?.Value;
    }
}

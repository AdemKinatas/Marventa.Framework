using Microsoft.AspNetCore.Http;

namespace Marventa.Framework.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, TenantResolver tenantResolver)
    {
        var tenantId = tenantResolver.ResolveTenantId();
        var tenantName = tenantResolver.ResolveTenantName();

        if (tenantId.HasValue && !string.IsNullOrEmpty(tenantName))
        {
            tenantContext.SetTenant(tenantId.Value, tenantName);
        }

        await _next(context);
    }
}

using Marventa.Framework.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Marventa.Framework.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MultiTenancyOptions _options;

    public TenantMiddleware(RequestDelegate next, IOptions<MultiTenancyOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (_options.ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var tenantId = ResolveTenantId(context);
        var tenantName = ResolveTenantName(context);

        if (!string.IsNullOrEmpty(tenantId))
        {
            if (Guid.TryParse(tenantId, out var id))
            {
                tenantContext.SetTenant(id, tenantName ?? tenantId);
            }
        }
        else if (_options.RequireTenant)
        {
            if (!string.IsNullOrEmpty(_options.DefaultTenantId))
            {
                if (Guid.TryParse(_options.DefaultTenantId, out var defaultId))
                {
                    tenantContext.SetTenant(defaultId, _options.DefaultTenantId);
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Tenant information is required.");
                return;
            }
        }

        await _next(context);
    }

    private string? ResolveTenantId(HttpContext context)
    {
        return _options.Strategy switch
        {
            TenantResolutionStrategy.Header => context.Request.Headers[_options.HeaderName].FirstOrDefault(),
            TenantResolutionStrategy.QueryParameter => context.Request.Query[_options.QueryParameterName].FirstOrDefault(),
            TenantResolutionStrategy.Subdomain => ExtractSubdomain(context),
            TenantResolutionStrategy.Claim => context.User.FindFirstValue("TenantId"),
            _ => null
        };
    }

    private string? ResolveTenantName(HttpContext context)
    {
        return _options.Strategy switch
        {
            TenantResolutionStrategy.Header => context.Request.Headers["X-Tenant-Name"].FirstOrDefault(),
            TenantResolutionStrategy.Claim => context.User.FindFirstValue("TenantName"),
            _ => null
        };
    }

    private string? ExtractSubdomain(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');

        if (parts.Length >= 3)
        {
            return _options.SubdomainPosition?.ToLower() == "prefix" ? parts[0] : parts[^3];
        }

        return null;
    }
}

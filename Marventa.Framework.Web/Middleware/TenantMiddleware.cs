using Microsoft.AspNetCore.Http;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Marventa.Framework.Infrastructure.Multitenancy;

namespace Marventa.Framework.Web.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;
    private readonly TenantOptions _options;

    public TenantMiddleware(
        RequestDelegate next,
        ILogger<TenantMiddleware> logger,
        IOptions<TenantOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, ITenantResolver<HttpContext> tenantResolver, ITenantContext tenantContext)
    {
        try
        {
            var tenant = await tenantResolver.ResolveAsync(context);

            if (tenant != null)
            {
                tenantContext.SetTenant(tenant);
                context.Items["TenantId"] = tenant.Id;
                context.Items["TenantName"] = tenant.Name;

                _logger.LogDebug("Tenant resolved: {TenantId}", tenant.Id);
            }
            else if (_options.RequireTenant)
            {
                _logger.LogWarning("No tenant resolved and RequireTenant is enabled for path: {Path}", context.Request.Path);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Tenant is required but not specified");
                return;
            }
            else if (!string.IsNullOrEmpty(_options.DefaultTenantId))
            {
                tenantContext.SetTenant(_options.DefaultTenantId);
                context.Items["TenantId"] = _options.DefaultTenantId;
                _logger.LogDebug("Using default tenant: {TenantId}", _options.DefaultTenantId);
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in tenant middleware");

            if (_options.RequireTenant)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Tenant resolution failed");
                return;
            }

            await _next(context);
        }
        finally
        {
            tenantContext.Clear();
        }
    }
}
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marventa.Framework.Web.Middleware;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationMiddleware> _logger;
    private readonly CorrelationOptions _options;

    public CorrelationMiddleware(
        RequestDelegate next,
        ILogger<CorrelationMiddleware> logger,
        IOptions<CorrelationOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, ICorrelationContext correlationContext, ITenantContext tenantContext)
    {
        // Extract or generate correlation ID
        var correlationId = ExtractCorrelationId(context);
        correlationContext.SetCorrelationId(correlationId);

        // Set tenant ID if available
        if (tenantContext.HasTenant)
        {
            correlationContext.SetTenantId(tenantContext.TenantId!);
        }

        // Extract user ID from claims if available
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value
                        ?? context.User.FindFirst("user_id")?.Value
                        ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                correlationContext.SetUserId(userId);
            }
        }

        // Add correlation ID to response headers
        context.Response.Headers[_options.ResponseHeaderName] = correlationId;

        // Add correlation ID to logging context
        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TenantId"] = tenantContext.TenantId ?? "unknown",
            ["UserId"] = correlationContext.UserId ?? "anonymous"
        });

        // Set activity tags
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("correlation.id", correlationId);
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("http.url", context.Request.GetDisplayUrl());
            activity.SetTag("http.user_agent", context.Request.Headers.UserAgent.ToString());
            activity.SetTag("tenant.id", tenantContext.TenantId);
            activity.SetTag("user.id", correlationContext.UserId);
        }

        try
        {
            await _next(context);

            // Add final status code
            activity?.SetTag("http.status_code", context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("http.status_code", 500);
            throw;
        }
    }

    private string ExtractCorrelationId(HttpContext context)
    {
        // Try to get from header
        if (context.Request.Headers.TryGetValue(_options.RequestHeaderName, out var headerValues))
        {
            var correlationId = headerValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(correlationId) && IsValidCorrelationId(correlationId))
            {
                return correlationId;
            }
        }

        // Try to get from trace parent (W3C trace context)
        if (context.Request.Headers.TryGetValue("traceparent", out var traceParent))
        {
            var trace = traceParent.FirstOrDefault();
            if (!string.IsNullOrEmpty(trace))
            {
                // Extract trace ID from traceparent header (first 32 chars after version)
                var parts = trace.Split('-');
                if (parts.Length >= 2)
                {
                    return parts[1];
                }
            }
        }

        // Generate new correlation ID
        return Guid.NewGuid().ToString("N");
    }

    private bool IsValidCorrelationId(string correlationId)
    {
        return !string.IsNullOrWhiteSpace(correlationId)
               && correlationId.Length >= _options.MinLength
               && correlationId.Length <= _options.MaxLength;
    }
}

public class CorrelationOptions
{
    public const string SectionName = "Correlation";

    public string RequestHeaderName { get; set; } = "X-Correlation-ID";
    public string ResponseHeaderName { get; set; } = "X-Correlation-ID";
    public int MinLength { get; set; } = 8;
    public int MaxLength { get; set; } = 64;
    public bool IncludeInLogs { get; set; } = true;
    public bool PropagateToActivities { get; set; } = true;
}
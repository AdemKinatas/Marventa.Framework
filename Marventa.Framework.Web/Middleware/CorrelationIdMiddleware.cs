using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.Middleware;

/// <summary>
/// Correlation ID middleware for request tracing
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CORRELATION_ID_HEADER = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Add to response headers
        context.Response.Headers[CORRELATION_ID_HEADER] = correlationId;

        // Add to logging context
        using var scope = _logger.BeginScope("CorrelationId: {CorrelationId}", correlationId);

        // Store in items for access in controllers
        context.Items["CorrelationId"] = correlationId;

        await _next(context);
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // Try to get from request header first
        if (context.Request.Headers.TryGetValue(CORRELATION_ID_HEADER, out var correlationId)
            && !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        // Generate new correlation ID
        return Guid.NewGuid().ToString("D");
    }
}

/// <summary>
/// Extension methods for accessing correlation ID
/// </summary>
public static class CorrelationIdExtensions
{
    public static string? GetCorrelationId(this HttpContext context)
    {
        return context.Items["CorrelationId"] as string;
    }
}
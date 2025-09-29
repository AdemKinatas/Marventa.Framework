using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Web.Models;

namespace Marventa.Framework.Web.Middleware;

/// <summary>
/// Unified Marventa middleware combining all enterprise features in one optimized component
/// </summary>
public class MarventaUnifiedMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MarventaUnifiedMiddleware> _logger;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly MarventaFrameworkOptions _options;

    public MarventaUnifiedMiddleware(
        RequestDelegate next,
        ILogger<MarventaUnifiedMiddleware> logger,
        IMemoryCache cache,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
        _configuration = configuration;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // 1. CORRELATION ID - Always enabled for tracking
        var correlationId = GetOrCreateCorrelationId(context);
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        context.Items["CorrelationId"] = correlationId;

        using var scope = _logger.BeginScope("CorrelationId: {CorrelationId}", correlationId);

        try
        {
            // 2. RATE LIMITING (if enabled)
            if (_options.EnableRateLimiting && await IsRateLimited(context))
            {
                await HandleRateLimitExceeded(context);
                return;
            }

            // 3. API KEY AUTH (if enabled)
            if (_options.EnableApiKeys && !await ValidateApiKey(context))
            {
                await HandleUnauthorized(context, "Invalid or missing API key");
                return;
            }

            // 4. SECURITY HEADERS - Always enabled
            AddSecurityHeaders(context);

            // 5. REQUEST LOGGING (if enabled)
            if (_options.EnableLogging)
            {
                LogRequest(context);
            }

            // 6. EXECUTE NEXT MIDDLEWARE
            await _next(context);

        }
        catch (Exception ex)
        {
            // 7. EXCEPTION HANDLING (if enabled)
            if (_options.EnableExceptionHandling)
            {
                await HandleException(context, ex, correlationId);
            }
            else
            {
                throw; // Let default handler manage it
            }
        }
        finally
        {
            stopwatch.Stop();

            // 8. RESPONSE LOGGING (if enabled)
            if (_options.EnableLogging)
            {
                LogResponse(context, stopwatch.ElapsedMilliseconds, correlationId);
            }
        }
    }

    #region Private Methods

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId)
            && !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }
        return Guid.NewGuid().ToString("D");
    }

    private Task<bool> IsRateLimited(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var key = $"rate_limit_{clientId}";

        // Get rate limiting configuration from appsettings with fallback to options
        var maxRequests = _configuration.GetValue<int?>("Marventa:RateLimit:MaxRequests") ?? _options.MiddlewareOptions.RateLimiting.MaxRequests;
        var windowMinutes = _configuration.GetValue<int?>("Marventa:RateLimit:WindowMinutes") ?? _options.MiddlewareOptions.RateLimiting.WindowMinutes;

        if (_cache.TryGetValue(key, out RateLimitCounter? counter))
        {
            if (counter!.RequestCount >= maxRequests)
            {
                _logger.LogWarning("🚫 Rate limit exceeded for {ClientId}: {Count}/{Max}",
                    clientId, counter.RequestCount, maxRequests);
                return Task.FromResult(true);
            }
            counter.RequestCount++;
            _cache.Set(key, counter, counter.ResetTime);
        }
        else
        {
            var resetTime = DateTime.UtcNow.AddMinutes(windowMinutes);
            _cache.Set(key, new RateLimitCounter { RequestCount = 1, ResetTime = resetTime }, resetTime);
        }

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] =
            Math.Max(0, maxRequests - (counter?.RequestCount ?? 1)).ToString();

        return Task.FromResult(false);
    }

    private Task<bool> ValidateApiKey(HttpContext context)
    {
        // Skip for health checks and documentation
        if (ShouldSkipAuth(context.Request.Path))
            return Task.FromResult(true);

        if (!context.Request.Headers.TryGetValue("X-API-Key", out var providedApiKey))
            return Task.FromResult(false);

        // Get valid API keys from user's configuration
        var validApiKeys = _configuration.GetSection("Marventa:ApiKeys").Get<string[]>();
        var primaryApiKey = _configuration["Marventa:ApiKey"];

        if (validApiKeys?.Any() == true)
        {
            return Task.FromResult(Array.Exists(validApiKeys, key => key == providedApiKey));
        }

        if (!string.IsNullOrEmpty(primaryApiKey))
        {
            return Task.FromResult(primaryApiKey == providedApiKey);
        }

        // If no API keys configured, reject all requests
        _logger.LogWarning("No API keys configured in appsettings. Please add Marventa:ApiKey or Marventa:ApiKeys section.");
        return Task.FromResult(false);
    }

    private bool ShouldSkipAuth(PathString path)
    {
        // Get skip paths from configuration, with defaults
        var configuredSkipPaths = _configuration.GetSection("Marventa:ApiKey:SkipPaths").Get<string[]>();
        var defaultSkipPaths = new[] { "/health", "/swagger", "/api/docs", "/favicon.ico" };

        var skipPaths = configuredSkipPaths?.Length > 0 ? configuredSkipPaths : defaultSkipPaths;
        return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath));
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["X-XSS-Protection"] = "1; mode=block";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["X-Powered-By"] = "Marventa.Framework";
    }

    private void LogRequest(HttpContext context)
    {
        _logger.LogInformation("🔄 Request: {Method} {Path} - User: {User} - IP: {IP}",
            context.Request.Method,
            context.Request.Path,
            context.User?.Identity?.Name ?? "Anonymous",
            context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
    }

    private void LogResponse(HttpContext context, long durationMs, string correlationId)
    {
        var level = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

        _logger.Log(level, "✅ Response: {StatusCode} - {Duration}ms - Size: {Size}b - Correlation: {CorrelationId}",
            context.Response.StatusCode,
            durationMs,
            context.Response.ContentLength ?? 0,
            correlationId);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
            return $"user_{userId}";

        var ip = context.Connection.RemoteIpAddress?.ToString();
        return $"ip_{ip ?? "unknown"}";
    }

    private async Task HandleRateLimitExceeded(HttpContext context)
    {
        context.Response.StatusCode = 429;
        context.Response.ContentType = "application/json";
        context.Response.Headers["Retry-After"] = "900"; // 15 minutes

        var response = new
        {
            error = "Rate limit exceeded",
            message = "Too many requests. Please try again later.",
            retryAfter = 900
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Unauthorized",
            message = message,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleException(HttpContext context, Exception exception, string correlationId)
    {
        _logger.LogError(exception, "❌ Unhandled exception - Correlation: {CorrelationId}", correlationId);

        context.Response.StatusCode = exception switch
        {
            ArgumentException => 400,
            UnauthorizedAccessException => 401,
            KeyNotFoundException => 404,
            _ => 500
        };

        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "An error occurred",
            message = context.Response.StatusCode == 500
                ? "Internal server error. Please contact support."
                : exception.Message,
            correlationId = correlationId,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    #endregion
}
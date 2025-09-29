using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;
using Marventa.Framework.Web.Models;

namespace Marventa.Framework.Web.Middleware;

/// <summary>
/// Rate limiting middleware to prevent API abuse
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<RateLimitingMiddleware> logger,
        RateLimitOptions? options = null)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _options = options ?? new RateLimitOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var key = $"rate_limit_{clientId}";

        if (_cache.TryGetValue(key, out RateLimitCounter? counter))
        {
            if (counter!.RequestCount >= _options.MaxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
                await HandleRateLimitExceeded(context);
                return;
            }

            counter.RequestCount++;
            _cache.Set(key, counter, counter.ResetTime);
        }
        else
        {
            var resetTime = DateTime.UtcNow.Add(_options.Window);
            _cache.Set(key, new RateLimitCounter
            {
                RequestCount = 1,
                ResetTime = resetTime
            }, resetTime);
        }

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] =
            Math.Max(0, _options.MaxRequests - (counter?.RequestCount ?? 1)).ToString();

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID first, fallback to IP
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
            return $"user_{userId}";

        var ip = context.Connection.RemoteIpAddress?.ToString();
        return $"ip_{ip ?? "unknown"}";
    }

    private async Task HandleRateLimitExceeded(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Rate limit exceeded",
            message = $"Too many requests. Limit: {_options.MaxRequests} per {_options.Window.TotalMinutes} minutes",
            retryAfter = _options.Window.TotalSeconds
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}
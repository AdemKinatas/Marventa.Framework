using System;
using System.Threading.Tasks;
using Marventa.Framework.Core.Constants;
using Marventa.Framework.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.RateLimiting;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, RateLimitOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context, ICacheService cacheService)
    {
        if (!_options.EnableRateLimiting)
        {
            await _next(context);
            return;
        }

        var clientIdentifier = GetClientIdentifier(context);
        var cacheKey = $"rate_limit:{clientIdentifier}";

        var currentRequests = await cacheService.GetAsync<int?>(cacheKey) ?? 0;

        if (currentRequests >= _options.MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientIdentifier);

            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.Headers[HttpHeaders.RATE_LIMIT_REMAINING] = "0";
            context.Response.Headers[HttpHeaders.RATE_LIMIT_RESET] =
                DateTimeOffset.UtcNow.Add(_options.WindowSize).ToUnixTimeSeconds().ToString();

            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }

        // Increment request count
        await cacheService.SetAsync(cacheKey, currentRequests + 1, _options.WindowSize);

        // Add rate limit headers
        context.Response.Headers[HttpHeaders.RATE_LIMIT_REMAINING] = (_options.MaxRequests - currentRequests - 1).ToString();
        context.Response.Headers[HttpHeaders.RATE_LIMIT_RESET] =
            DateTimeOffset.UtcNow.Add(_options.WindowSize).ToUnixTimeSeconds().ToString();

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID first, fall back to IP address
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
            return userId;

        // Get client IP address
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        return !string.IsNullOrEmpty(forwardedFor) ? forwardedFor : clientIp ?? "unknown";
    }
}

public class RateLimitOptions
{
    public bool EnableRateLimiting { get; set; } = true;
    public int MaxRequests { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
}
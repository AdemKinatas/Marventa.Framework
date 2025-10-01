using Marventa.Framework.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;

namespace Marventa.Framework.Security.RateLimiting;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly RateLimitingOptions _options;

    public RateLimiterMiddleware(RequestDelegate next, IMemoryCache cache, IOptions<RateLimitingOptions> options)
    {
        _next = next;
        _cache = cache;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (_options.ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var clientIdentifier = GetClientIdentifier(context);

        if (_options.ExcludedIpAddresses.Contains(clientIdentifier))
        {
            await _next(context);
            return;
        }

        var cacheKey = $"RateLimit_{clientIdentifier}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        var remaining = _options.RequestLimit - requestCount;
        var timeWindow = TimeSpan.FromSeconds(_options.TimeWindowSeconds);

        if (_options.ReturnRateLimitHeaders)
        {
            context.Response.Headers["X-RateLimit-Limit"] = _options.RequestLimit.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remaining - 1).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(timeWindow).ToUnixTimeSeconds().ToString();
        }

        if (requestCount >= _options.RequestLimit)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            var errorMessage = _options.CustomErrorMessage ?? "Rate limit exceeded. Please try again later.";
            await context.Response.WriteAsync(errorMessage);
            return;
        }

        _cache.Set(cacheKey, requestCount + 1, timeWindow);
        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        return _options.Strategy switch
        {
            RateLimitStrategy.IpAddress => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            RateLimitStrategy.UserId => context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
            RateLimitStrategy.ApiKey => context.Request.Headers["X-Api-Key"].ToString() ?? "no-key",
            RateLimitStrategy.CustomHeader => context.Request.Headers[_options.CustomHeaderName ?? "X-Client-Id"].ToString() ?? "no-header",
            _ => context.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        };
    }
}

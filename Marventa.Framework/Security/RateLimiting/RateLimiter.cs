using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Marventa.Framework.Security.RateLimiting;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;

    public RateLimiterMiddleware(RequestDelegate next, IMemoryCache cache, int requestLimit = 100, int timeWindowSeconds = 60)
    {
        _next = next;
        _cache = cache;
        _requestLimit = requestLimit;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"RateLimit_{clientIp}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        if (requestCount >= _requestLimit)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        _cache.Set(cacheKey, requestCount + 1, _timeWindow);
        await _next(context);
    }
}

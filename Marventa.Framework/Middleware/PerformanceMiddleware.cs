using Marventa.Framework.Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Marventa.Framework.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        // Log performance
        if (stopwatch.ElapsedMilliseconds > MiddlewareConstants.SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                LogMessages.SlowRequest,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }

        // Response timing'i context.Items'a ekle (ApiResponse kullanılabilir)
        context.Items[MiddlewareConstants.ResponseTimeMsProperty] = stopwatch.ElapsedMilliseconds;
    }
}

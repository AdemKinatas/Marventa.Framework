using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace Marventa.Framework.Web.Middleware;

/// <summary>
/// Detailed request and response logging middleware
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestBody = string.Empty;

        // Log request
        if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        _logger.LogInformation("HTTP Request: {Method} {Path} - Content Length: {ContentLength} - User: {User}",
            context.Request.Method,
            context.Request.Path,
            context.Request.ContentLength ?? 0,
            context.User?.Identity?.Name ?? "Anonymous");

        if (!string.IsNullOrEmpty(requestBody) && requestBody.Length < 1000) // Don't log huge payloads
        {
            _logger.LogDebug("Request Body: {RequestBody}", requestBody);
        }

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log response
            var responseBodyText = string.Empty;
            if (responseBody.Length > 0)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(responseBody);
                responseBodyText = await reader.ReadToEndAsync();

                if (responseBodyText.Length < 1000) // Don't log huge responses
                {
                    _logger.LogDebug("Response Body: {ResponseBody}", responseBodyText);
                }

                responseBody.Seek(0, SeekOrigin.Begin);
            }

            _logger.LogInformation("HTTP Response: {StatusCode} - Duration: {Duration}ms - Size: {Size} bytes",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                responseBody.Length);

            // Copy the response back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
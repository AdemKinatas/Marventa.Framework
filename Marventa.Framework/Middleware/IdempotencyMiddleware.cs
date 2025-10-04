using Marventa.Framework.Features.Idempotency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Marventa.Framework.Middleware;

/// <summary>
/// Middleware that provides idempotency support for HTTP requests.
/// Uses the Idempotency-Key header to cache and replay responses for duplicate requests.
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;
    private const string IdempotencyKeyHeader = "Idempotency-Key";

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger instance.</param>
    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="idempotencyService">The idempotency service.</param>
    public async Task InvokeAsync(HttpContext context, IIdempotencyService idempotencyService)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (idempotencyService == null)
            throw new ArgumentNullException(nameof(idempotencyService));

        // Only apply to POST, PUT, PATCH, DELETE methods
        if (!ShouldApplyIdempotency(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // Check if idempotency key is present
        if (!context.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var idempotencyKey) ||
            string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await _next(context);
            return;
        }

        var key = idempotencyKey.ToString();

        // Check if request has already been processed
        var cachedResponse = await idempotencyService.GetResponseAsync(key, context.RequestAborted);

        if (cachedResponse != null)
        {
            _logger.LogInformation("Returning cached response for idempotency key: {IdempotencyKey}", key);
            await WriteCachedResponseAsync(context, cachedResponse);
            return;
        }

        // Capture the response
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Only cache successful responses (2xx status codes)
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await CacheResponseAsync(context, idempotencyService, key, responseBody);
            }

            // Copy the response back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    /// <summary>
    /// Determines if idempotency should be applied to the request method.
    /// </summary>
    private static bool ShouldApplyIdempotency(string method)
    {
        return method == HttpMethods.Post ||
               method == HttpMethods.Put ||
               method == HttpMethods.Patch ||
               method == HttpMethods.Delete;
    }

    /// <summary>
    /// Writes a cached response to the HTTP response.
    /// </summary>
    private async Task WriteCachedResponseAsync(HttpContext context, IdempotentResponse cachedResponse)
    {
        context.Response.StatusCode = cachedResponse.StatusCode;

        if (!string.IsNullOrEmpty(cachedResponse.ContentType))
        {
            context.Response.ContentType = cachedResponse.ContentType;
        }

        // Restore headers (excluding some that shouldn't be cached)
        foreach (var header in cachedResponse.Headers)
        {
            if (!ShouldSkipHeader(header.Key))
            {
                context.Response.Headers[header.Key] = header.Value;
            }
        }

        if (!string.IsNullOrEmpty(cachedResponse.Body))
        {
            await context.Response.WriteAsync(cachedResponse.Body);
        }
    }

    /// <summary>
    /// Caches the current response.
    /// </summary>
    private async Task CacheResponseAsync(
        HttpContext context,
        IIdempotencyService idempotencyService,
        string key,
        MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(responseBody).ReadToEndAsync();

        var headers = new Dictionary<string, string[]>();
        foreach (var header in context.Response.Headers)
        {
            if (!ShouldSkipHeader(header.Key))
            {
                var values = header.Value.ToArray();
                headers[header.Key] = values.Where(v => v != null).ToArray()!;
            }
        }

        var cachedResponse = new IdempotentResponse
        {
            StatusCode = context.Response.StatusCode,
            Body = body,
            Headers = headers,
            ContentType = context.Response.ContentType
        };

        await idempotencyService.SetResponseAsync(key, cachedResponse, context.RequestAborted);

        _logger.LogInformation("Cached response for idempotency key: {IdempotencyKey}", key);
    }

    /// <summary>
    /// Determines if a header should be skipped when caching.
    /// </summary>
    private static bool ShouldSkipHeader(string headerName)
    {
        // Skip headers that shouldn't be cached
        var headersToSkip = new[]
        {
            "Date",
            "Server",
            "X-Request-Id",
            "X-Correlation-Id"
        };

        return headersToSkip.Contains(headerName, StringComparer.OrdinalIgnoreCase);
    }
}

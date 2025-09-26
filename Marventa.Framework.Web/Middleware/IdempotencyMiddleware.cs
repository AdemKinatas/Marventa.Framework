using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Infrastructure.Idempotency;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marventa.Framework.Web.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;
    private readonly IdempotencyOptions _options;

    public IdempotencyMiddleware(
        RequestDelegate next,
        ILogger<IdempotencyMiddleware> logger,
        IOptions<IdempotencyOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, IIdempotencyService idempotencyService)
    {
        if (!_options.IdempotentMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        if (_options.IgnoredPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(_options.HeaderName, out var idempotencyKeyValues))
        {
            if (_options.RequireIdempotencyKey)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Missing required header: {_options.HeaderName}");
                return;
            }

            await _next(context);
            return;
        }

        var idempotencyKey = idempotencyKeyValues.FirstOrDefault();
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"Empty idempotency key in header: {_options.HeaderName}");
            return;
        }

        try
        {
            var contextKey = BuildIdempotencyKey(context, idempotencyKey);

            var result = await idempotencyService.ProcessAsync(contextKey, async () =>
            {
                var originalBodyStream = context.Response.Body;
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBodyStream);

                return new IdempotentResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Body = responseBody,
                    Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    ContentType = context.Response.ContentType
                };
            });

            if (result.IsFromCache)
            {
                _logger.LogDebug("Returning cached response for idempotency key: {Key}", idempotencyKey);

                context.Response.Headers["X-Idempotency-Replayed"] = "true";
                context.Response.Headers["X-Idempotency-Processed-At"] = result.ProcessedAt.ToString("O");
            }

            if (result.Result is IdempotentResponse response)
            {
                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = response.ContentType;

                foreach (var header in response.Headers)
                {
                    if (!context.Response.Headers.ContainsKey(header.Key))
                    {
                        context.Response.Headers[header.Key] = header.Value;
                    }
                }

                if (!string.IsNullOrEmpty(response.Body) && !result.IsFromCache)
                {
                    await context.Response.WriteAsync(response.Body);
                }
                else if (!string.IsNullOrEmpty(response.Body))
                {
                    var bytes = Encoding.UTF8.GetBytes(response.Body);
                    await context.Response.Body.WriteAsync(bytes);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing idempotent request with key: {Key}", idempotencyKey);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error processing idempotent request");
        }
    }

    private string BuildIdempotencyKey(HttpContext context, string idempotencyKey)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{context.Request.Method}:");
        keyBuilder.Append($"{context.Request.Path}:");

        if (context.Request.Method == "GET" && context.Request.Query.Count > 0)
        {
            var sortedQuery = context.Request.Query
                .OrderBy(q => q.Key)
                .Select(q => $"{q.Key}={string.Join(",", q.Value.AsEnumerable())}")
                .ToArray();
            keyBuilder.Append($"?{string.Join("&", sortedQuery)}:");
        }

        keyBuilder.Append(idempotencyKey);
        return keyBuilder.ToString();
    }

    private class IdempotentResponse
    {
        public int StatusCode { get; set; }
        public string Body { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
        public string? ContentType { get; set; }
    }
}
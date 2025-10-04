using Marventa.Framework.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Marventa.Framework.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with sensitive data masking.
/// Helps with debugging and auditing by capturing complete request/response information.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly LoggingOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestResponseLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="options">Logging options.</param>
    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IOptions<LoggingOptions> options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (!_options.EnableRequestResponseLogging)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Log request
        await LogRequestAsync(context, requestId);

        // Capture response
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Log response
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds, responseBody);

            // Copy the response back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{RequestId}] Exception occurred after {ElapsedMs}ms", requestId, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    /// <summary>
    /// Logs the HTTP request.
    /// </summary>
    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;

        // Enable buffering so the request body can be read multiple times
        request.EnableBuffering();

        var logBuilder = new StringBuilder();
        logBuilder.AppendLine($"[{requestId}] HTTP Request:");
        logBuilder.AppendLine($"Method: {request.Method}");
        logBuilder.AppendLine($"Path: {request.Path}{request.QueryString}");
        logBuilder.AppendLine($"Protocol: {request.Protocol}");

        // Log headers
        if (_options.LogRequestHeaders && request.Headers.Any())
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in request.Headers)
            {
                var value = IsSensitiveHeader(header.Key)
                    ? "***MASKED***"
                    : string.Join(", ", header.Value.ToArray());
                logBuilder.AppendLine($"  {header.Key}: {value}");
            }
        }

        // Log body
        if (_options.LogRequestBody && request.ContentLength > 0 && IsLoggableContentType(request.ContentType))
        {
            request.Body.Position = 0;
            var body = await ReadBodyAsync(request.Body);
            request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body))
            {
                var maskedBody = MaskSensitiveData(body);
                logBuilder.AppendLine($"Body: {maskedBody}");
            }
        }

        _logger.LogInformation(logBuilder.ToString());
    }

    /// <summary>
    /// Logs the HTTP response.
    /// </summary>
    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs, MemoryStream responseBody)
    {
        var response = context.Response;

        var logBuilder = new StringBuilder();
        logBuilder.AppendLine($"[{requestId}] HTTP Response ({elapsedMs}ms):");
        logBuilder.AppendLine($"Status: {response.StatusCode}");

        // Log headers
        if (_options.LogResponseHeaders && response.Headers.Any())
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in response.Headers)
            {
                var value = IsSensitiveHeader(header.Key)
                    ? "***MASKED***"
                    : string.Join(", ", header.Value.ToArray());
                logBuilder.AppendLine($"  {header.Key}: {value}");
            }
        }

        // Log body
        if (_options.LogResponseBody && responseBody.Length > 0 && IsLoggableContentType(response.ContentType))
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var body = await ReadBodyAsync(responseBody);

            if (!string.IsNullOrEmpty(body))
            {
                var maskedBody = MaskSensitiveData(body);
                logBuilder.AppendLine($"Body: {maskedBody}");
            }
        }

        _logger.LogInformation(logBuilder.ToString());
    }

    /// <summary>
    /// Reads the body from a stream.
    /// </summary>
    private async Task<string> ReadBodyAsync(Stream stream)
    {
        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 4096,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();

        // Truncate if too large
        if (body.Length > _options.MaxBodyLogSize)
        {
            body = body.Substring(0, _options.MaxBodyLogSize) + "... [TRUNCATED]";
        }

        return body;
    }

    /// <summary>
    /// Checks if a header is sensitive and should be masked.
    /// </summary>
    private bool IsSensitiveHeader(string headerName)
    {
        return _options.SensitiveHeaders.Any(h =>
            h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Masks sensitive data in the body.
    /// </summary>
    private string MaskSensitiveData(string body)
    {
        if (string.IsNullOrEmpty(body))
            return body;

        try
        {
            // Try to parse as JSON and mask sensitive fields
            var jsonDocument = JsonDocument.Parse(body);
            var maskedBody = MaskJsonFields(body);
            return maskedBody;
        }
        catch (JsonException)
        {
            // Not JSON, use regex to mask sensitive fields
            return MaskWithRegex(body);
        }
    }

    /// <summary>
    /// Masks sensitive fields in JSON body.
    /// </summary>
    private string MaskJsonFields(string json)
    {
        foreach (var field in _options.SensitiveBodyFields)
        {
            // Match "field": "value" or "field":"value"
            var pattern = $@"""({field})""\s*:\s*""([^""]*)""";
            json = Regex.Replace(json, pattern, $@"""{field}"":""***MASKED***""", RegexOptions.IgnoreCase);
        }

        return json;
    }

    /// <summary>
    /// Masks sensitive fields using regex for non-JSON bodies.
    /// </summary>
    private string MaskWithRegex(string body)
    {
        foreach (var field in _options.SensitiveBodyFields)
        {
            // Match field=value or field: value
            var pattern = $@"({field})\s*[=:]\s*([^\s&,;]+)";
            body = Regex.Replace(body, pattern, $@"{field}=***MASKED***", RegexOptions.IgnoreCase);
        }

        return body;
    }

    /// <summary>
    /// Checks if the content type is loggable (text-based).
    /// </summary>
    private static bool IsLoggableContentType(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        var loggableTypes = new[]
        {
            "application/json",
            "application/xml",
            "text/",
            "application/x-www-form-urlencoded"
        };

        return loggableTypes.Any(type =>
            contentType.StartsWith(type, StringComparison.OrdinalIgnoreCase));
    }
}

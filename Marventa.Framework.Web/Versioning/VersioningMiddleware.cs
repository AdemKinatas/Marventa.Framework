using System;
using System.Threading.Tasks;
using Marventa.Framework.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.Versioning;

public class VersioningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<VersioningMiddleware> _logger;
    private readonly ApiVersioningOptions _options;

    public VersioningMiddleware(RequestDelegate next, ILogger<VersioningMiddleware> logger, ApiVersioningOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var version = ExtractVersion(context);

        if (string.IsNullOrEmpty(version) && _options.AssumeDefaultVersionWhenUnspecified)
        {
            version = _options.DefaultVersion;
        }

        if (!string.IsNullOrEmpty(version))
        {
            context.Items["ApiVersion"] = version;
            context.Response.Headers[HttpHeaders.API_VERSION] = version;
            _logger.LogDebug("API Version set to: {Version}", version);
        }

        await _next(context);
    }

    private string? ExtractVersion(HttpContext context)
    {
        return _options.ApiVersionReader switch
        {
            ApiVersionReader.Header => context.Request.Headers[_options.HeaderName].FirstOrDefault(),
            ApiVersionReader.QueryParameter => context.Request.Query[_options.QueryParameterName].FirstOrDefault(),
            ApiVersionReader.UrlSegment => ExtractFromUrlSegment(context),
            ApiVersionReader.MediaType => ExtractFromMediaType(context),
            _ => null
        };
    }

    private string? ExtractFromUrlSegment(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path?.Contains("/api/v") == true)
        {
            var segments = path.Split('/');
            foreach (var segment in segments)
            {
                if (segment.StartsWith("v") && segment.Length > 1)
                {
                    return segment[1..];
                }
            }
        }
        return null;
    }

    private string? ExtractFromMediaType(HttpContext context)
    {
        var acceptHeader = context.Request.Headers["Accept"].FirstOrDefault();
        if (!string.IsNullOrEmpty(acceptHeader) && acceptHeader.Contains("version="))
        {
            var versionStart = acceptHeader.IndexOf("version=", StringComparison.OrdinalIgnoreCase) + 8;
            var versionEnd = acceptHeader.IndexOf(';', versionStart);
            if (versionEnd == -1) versionEnd = acceptHeader.Length;

            return acceptHeader.Substring(versionStart, versionEnd - versionStart);
        }
        return null;
    }
}
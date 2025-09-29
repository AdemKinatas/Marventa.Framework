using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Marventa.Framework.Web.Middleware;

/// <summary>
/// API Key authentication middleware
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private const string API_KEY_HEADER = "X-API-Key";

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for certain paths
        if (ShouldSkipAuthentication(context.Request.Path))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            _logger.LogWarning("API Key missing for request: {Path}", context.Request.Path);
            await HandleUnauthorized(context, "API Key missing");
            return;
        }

        // Get valid API keys from user's configuration (same pattern as unified middleware)
        var validApiKeys = _configuration.GetSection("Marventa:ApiKeys").Get<string[]>();
        var primaryApiKey = _configuration["Marventa:ApiKey"];

        bool isValid = false;

        if (validApiKeys?.Any() == true)
        {
            isValid = Array.Exists(validApiKeys, key => key == extractedApiKey);
        }
        else if (!string.IsNullOrEmpty(primaryApiKey))
        {
            isValid = primaryApiKey == extractedApiKey;
        }
        else
        {
            _logger.LogError("No API keys configured in appsettings. Please add Marventa:ApiKey or Marventa:ApiKeys section.");
            await HandleUnauthorized(context, "API Key not configured");
            return;
        }

        if (!isValid)
        {
            _logger.LogWarning("Invalid API Key provided: {ApiKey}", extractedApiKey.ToString().Substring(0, Math.Min(10, extractedApiKey.ToString().Length)));
            await HandleUnauthorized(context, "Invalid API Key");
            return;
        }

        await _next(context);
    }

    private bool ShouldSkipAuthentication(PathString path)
    {
        // Get skip paths from configuration, with defaults
        var configuredSkipPaths = _configuration.GetSection("Marventa:ApiKey:SkipPaths").Get<string[]>();
        var defaultSkipPaths = new[]
        {
            "/health",
            "/swagger",
            "/api/docs",
            "/favicon.ico"
        };

        var skipPaths = configuredSkipPaths?.Length > 0 ? configuredSkipPaths : defaultSkipPaths;
        return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath));
    }

    private async Task HandleUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Unauthorized",
            message = message,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}
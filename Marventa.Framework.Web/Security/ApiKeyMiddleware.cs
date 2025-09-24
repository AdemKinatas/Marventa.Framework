using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.Security;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly string _apiKeyHeaderName;
    private readonly string[] _validApiKeys;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _apiKeyHeaderName = configuration["ApiKey:HeaderName"] ?? "X-API-Key";
        _validApiKeys = configuration.GetSection("ApiKey:ValidKeys").Get<string[]>() ?? Array.Empty<string>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip API key validation for certain endpoints
        if (ShouldSkipApiKeyValidation(context))
        {
            await _next(context);
            return;
        }

        var providedApiKey = context.Request.Headers[_apiKeyHeaderName].FirstOrDefault();

        if (string.IsNullOrEmpty(providedApiKey))
        {
            _logger.LogWarning("API Key missing from request");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is required");
            return;
        }

        if (!IsValidApiKey(providedApiKey))
        {
            _logger.LogWarning("Invalid API Key provided: {ApiKey}", providedApiKey);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        _logger.LogDebug("Valid API Key provided");
        await _next(context);
    }

    private bool IsValidApiKey(string apiKey)
    {
        return Array.Exists(_validApiKeys, key => string.Equals(key, apiKey, StringComparison.Ordinal));
    }

    private static bool ShouldSkipApiKeyValidation(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // Skip for health checks, swagger, etc.
        return path?.Contains("/health") == true ||
               path?.Contains("/swagger") == true ||
               path?.Contains("/api-docs") == true;
    }
}
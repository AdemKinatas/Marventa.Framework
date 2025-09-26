using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Attributes;
using Microsoft.AspNetCore.Http;

namespace Marventa.Framework.Application.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(
        IIdempotencyService idempotencyService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _idempotencyService = idempotencyService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var idempotentAttribute = GetIdempotentAttribute(request);
        if (idempotentAttribute == null)
        {
            return await next();
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var idempotencyKey = GetIdempotencyKey(idempotentAttribute, request, httpContext);

        if (string.IsNullOrEmpty(idempotencyKey))
        {
            if (idempotentAttribute.RequireKey)
            {
                throw new InvalidOperationException("Idempotency key is required but not provided");
            }
            return await next();
        }

        _logger.LogDebug("Processing idempotent request with key: {IdempotencyKey}", idempotencyKey);

        var expiration = TimeSpan.FromHours(idempotentAttribute.ExpirationHours);

        var result = await _idempotencyService.ProcessAsync(
            idempotencyKey,
            async () => await next(),
            expiration,
            cancellationToken);

        if (result.IsFromCache)
        {
            _logger.LogDebug("Returning cached result for idempotency key: {IdempotencyKey}", idempotencyKey);

            // Set response headers if available
            if (httpContext?.Response != null && result.Headers != null)
            {
                httpContext.Response.Headers["X-Idempotency-Cache"] = "HIT";
                foreach (var header in result.Headers)
                {
                    httpContext.Response.Headers[header.Key] = header.Value.ToString();
                }
            }
        }
        else
        {
            if (httpContext?.Response != null)
            {
                httpContext.Response.Headers["X-Idempotency-Cache"] = "MISS";
            }
        }

        return (TResponse)result.Result!;
    }

    private IdempotentAttribute? GetIdempotentAttribute(TRequest request)
    {
        // Check request type for attribute
        var requestType = request.GetType();
        return requestType.GetCustomAttribute<IdempotentAttribute>();
    }

    private string? GetIdempotencyKey(IdempotentAttribute attribute, TRequest request, HttpContext? httpContext)
    {
        // 1. Use template if provided
        if (!string.IsNullOrEmpty(attribute.KeyTemplate))
        {
            return ProcessKeyTemplate(attribute.KeyTemplate, request);
        }

        // 2. Use custom header name
        var headerName = attribute.HeaderName ?? "X-Idempotency-Key";
        if (httpContext?.Request.Headers.TryGetValue(headerName, out var headerValue) == true)
        {
            return headerValue.FirstOrDefault();
        }

        // 3. Generate from request content (for non-HTTP scenarios)
        if (httpContext == null)
        {
            return GenerateKeyFromRequest(request);
        }

        return null;
    }

    private string ProcessKeyTemplate(string template, TRequest request)
    {
        var result = template;
        var properties = request.GetType().GetProperties();

        foreach (var property in properties)
        {
            var placeholder = $"{{{property.Name}}}";
            if (result.Contains(placeholder))
            {
                var value = property.GetValue(request)?.ToString() ?? "";
                result = result.Replace(placeholder, value);
            }
        }

        return result;
    }

    private string GenerateKeyFromRequest(TRequest request)
    {
        // Generate a consistent key based on request content
        var json = System.Text.Json.JsonSerializer.Serialize(request);
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hash);
    }
}
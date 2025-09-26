using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Attributes;
using System.Text.Json;

namespace Marventa.Framework.Web.Filters;

public class IdempotencyFilter : IAsyncActionFilter
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger<IdempotencyFilter> _logger;

    public IdempotencyFilter(
        IIdempotencyService idempotencyService,
        ILogger<IdempotencyFilter> logger)
    {
        _idempotencyService = idempotencyService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var idempotentAttribute = GetIdempotentAttribute(context);
        if (idempotentAttribute == null)
        {
            await next();
            return;
        }

        var idempotencyKey = GetIdempotencyKey(idempotentAttribute, context);
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            if (idempotentAttribute.RequireKey)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    error = "Idempotency key is required",
                    detail = $"Please provide idempotency key in header: {idempotentAttribute.HeaderName ?? "X-Idempotency-Key"}"
                });
                return;
            }

            await next();
            return;
        }

        _logger.LogDebug("Processing idempotent action with key: {IdempotencyKey}", idempotencyKey);

        var expiration = TimeSpan.FromHours(idempotentAttribute.ExpirationHours);

        var result = await _idempotencyService.ProcessAsync(
            idempotencyKey,
            async () =>
            {
                var actionResult = await next();
                return ExtractResultData(actionResult);
            },
            expiration);

        if (result.IsFromCache)
        {
            _logger.LogDebug("Returning cached result for idempotency key: {IdempotencyKey}", idempotencyKey);
            context.HttpContext.Response.Headers["X-Idempotency-Cache"] = "HIT";
        }
        else
        {
            context.HttpContext.Response.Headers["X-Idempotency-Cache"] = "MISS";
        }

        if (result.Headers != null)
        {
            foreach (var header in result.Headers)
            {
                context.HttpContext.Response.Headers[header.Key] = header.Value.ToString();
            }
        }

        context.Result = CreateActionResult(result);
    }

    private IdempotentAttribute? GetIdempotentAttribute(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var actionAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttribute<IdempotentAttribute>();
            if (actionAttribute != null)
                return actionAttribute;

            var controllerAttribute = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<IdempotentAttribute>();
            if (controllerAttribute != null)
                return controllerAttribute;
        }

        return null;
    }

    private string? GetIdempotencyKey(IdempotentAttribute attribute, ActionExecutingContext context)
    {
        var headerName = attribute.HeaderName ?? "X-Idempotency-Key";

        if (context.HttpContext.Request.Headers.TryGetValue(headerName, out var headerValue))
        {
            var key = headerValue.FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                return key;
        }

        if (!string.IsNullOrEmpty(attribute.KeyTemplate))
        {
            return ProcessKeyTemplate(attribute.KeyTemplate, context);
        }

        return null;
    }

    private string ProcessKeyTemplate(string template, ActionExecutingContext context)
    {
        var result = template;

        foreach (var routeValue in context.RouteData.Values)
        {
            var placeholder = $"{{{routeValue.Key}}}";
            if (result.Contains(placeholder))
            {
                result = result.Replace(placeholder, routeValue.Value?.ToString() ?? "");
            }
        }

        foreach (var arg in context.ActionArguments)
        {
            var placeholder = $"{{{arg.Key}}}";
            if (result.Contains(placeholder))
            {
                result = result.Replace(placeholder, arg.Value?.ToString() ?? "");
            }
        }

        return result;
    }

    private object ExtractResultData(ActionExecutedContext actionResult)
    {
        if (actionResult.Result is ObjectResult objectResult)
        {
            return new
            {
                StatusCode = objectResult.StatusCode ?? 200,
                Value = objectResult.Value,
                Headers = actionResult.HttpContext.Response.Headers.ToDictionary(h => h.Key, h => (object)h.Value.ToString())
            };
        }

        if (actionResult.Result is StatusCodeResult statusCodeResult)
        {
            return new
            {
                StatusCode = statusCodeResult.StatusCode,
                Value = (object?)null,
                Headers = actionResult.HttpContext.Response.Headers.ToDictionary(h => h.Key, h => (object)h.Value.ToString())
            };
        }

        return new
        {
            StatusCode = 200,
            Value = actionResult.Result,
            Headers = actionResult.HttpContext.Response.Headers.ToDictionary(h => h.Key, h => (object)h.Value.ToString())
        };
    }

    private IActionResult CreateActionResult(IdempotencyResult result)
    {
        if (result.Result is JsonElement jsonElement)
        {
            var data = JsonSerializer.Deserialize<dynamic>(jsonElement);
            return new ObjectResult(data) { StatusCode = result.StatusCode };
        }

        return new ObjectResult(result.Result) { StatusCode = result.StatusCode };
    }
}
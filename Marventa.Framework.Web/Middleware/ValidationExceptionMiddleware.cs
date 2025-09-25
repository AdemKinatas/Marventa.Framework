using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Application.Exceptions;
using System.Diagnostics;

namespace Marventa.Framework.Web.Middleware;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            _logger.LogWarning(validationException, "Validation error occurred");
            await HandleValidationExceptionAsync(context, validationException);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/problem+json";

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var problemDetails = validationException.ToValidationProblemDetails(traceId);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "An error occurred while processing your request",
            status = (int)HttpStatusCode.InternalServerError,
            detail = "An unexpected error occurred. Please try again later.",
            traceId = traceId
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}
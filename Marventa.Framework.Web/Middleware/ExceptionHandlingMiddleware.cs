using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Marventa.Framework.Application.DTOs;
using Marventa.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            BusinessException businessEx => new ApiResponse
            {
                Success = false,
                Message = businessEx.Message,
                ErrorCode = businessEx.Code
            },
            ValidationException validationEx => new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationEx.Errors
            },
            NotFoundException notFoundEx => new ApiResponse
            {
                Success = false,
                Message = notFoundEx.Message,
                ErrorCode = "NOT_FOUND"
            },
            _ => new ApiResponse
            {
                Success = false,
                Message = "An internal server error occurred",
                ErrorCode = "INTERNAL_ERROR"
            }
        };

        context.Response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        BusinessException => (int)HttpStatusCode.BadRequest,
        ValidationException => (int)HttpStatusCode.BadRequest,
        NotFoundException => (int)HttpStatusCode.NotFound,
        _ => (int)HttpStatusCode.InternalServerError
    };
}
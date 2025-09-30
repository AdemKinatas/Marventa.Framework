using Marventa.Framework.ApiResponse;
using Marventa.Framework.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Marventa.Framework.ExceptionHandling;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = ResponseMessages.InternalServerError;
        var errorCode = ErrorCodes.InternalServerError;
        Dictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                errorCode = ErrorCodes.NotFound;
                break;

            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                errorCode = ErrorCodes.Unauthorized;
                break;

            case BusinessException businessException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                errorCode = businessException.ErrorCode;
                break;

            case Validation.ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = ResponseMessages.ValidationFailed;
                errorCode = ErrorCodes.ValidationError;
                errors = validationException.Errors;
                break;

            case FluentValidation.ValidationException fluentValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = ResponseMessages.ValidationFailed;
                errorCode = ErrorCodes.ValidationError;
                errors = fluentValidationException.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());
                break;
        }

        var response = ApiResponse<object>.ErrorResponse(message, errors);
        var result = JsonSerializer.Serialize(response);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(result);
    }
}

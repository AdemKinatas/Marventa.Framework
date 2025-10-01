using Marventa.Framework.ApiResponse;
using Marventa.Framework.Configuration;
using Marventa.Framework.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Marventa.Framework.ExceptionHandling;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly ExceptionHandlingOptions _options;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IOptions<ExceptionHandlingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (_options.LogExceptions)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = _options.DefaultErrorMessage;
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

        var response = CreateErrorResponse(message, errors, exception);
        var result = JsonSerializer.Serialize(response);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(result);
    }

    private object CreateErrorResponse(string message, Dictionary<string, string[]>? errors, Exception exception)
    {
        var response = ApiResponse<object>.ErrorResponse(message, errors);

        if (_options.IncludeExceptionDetails)
        {
            return new
            {
                response.Success,
                response.Message,
                response.Errors,
                ExceptionType = exception.GetType().Name,
                StackTrace = _options.IncludeStackTrace ? exception.StackTrace : null
            };
        }

        return response;
    }
}

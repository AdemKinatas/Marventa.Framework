using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Marventa.Framework.Web.Exceptions;

public class ValidationProblemDetails : ProblemDetails
{
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; } = new();

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    public ValidationProblemDetails()
    {
        Title = "One or more validation errors occurred";
        Status = StatusCodes.Status400BadRequest;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    }

    public ValidationProblemDetails(ValidationException validationException) : this()
    {
        ProcessValidationErrors(validationException.Errors);
    }

    public ValidationProblemDetails(IEnumerable<ValidationFailure> validationFailures) : this()
    {
        ProcessValidationErrors(validationFailures);
    }

    private void ProcessValidationErrors(IEnumerable<ValidationFailure> validationFailures)
    {
        var errors = validationFailures
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => ToCamelCase(group.Key),
                group => group.Select(x => x.ErrorMessage).ToArray()
            );

        Errors = errors;

        if (errors.Count > 0)
        {
            Detail = $"Validation failed for {errors.Count} field(s): {string.Join(", ", errors.Keys)}";
        }
    }

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 2)
            return input.ToLowerInvariant();

        return char.ToLowerInvariant(input[0]) + input[1..];
    }
}

public static class ValidationExtensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this ValidationException validationException, string? traceId = null)
    {
        var problemDetails = new ValidationProblemDetails(validationException)
        {
            TraceId = traceId
        };

        return problemDetails;
    }

    public static ValidationProblemDetails ToValidationProblemDetails(this IEnumerable<ValidationFailure> validationFailures, string? traceId = null)
    {
        var problemDetails = new ValidationProblemDetails(validationFailures)
        {
            TraceId = traceId
        };

        return problemDetails;
    }
}
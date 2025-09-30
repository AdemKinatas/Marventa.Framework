using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation before execution
/// </summary>
/// <typeparam name="TRequest">The type of request being validated</typeparam>
/// <typeparam name="TResponse">The type of response returned after validation</typeparam>
/// <remarks>
/// This behavior executes FIRST in the pipeline before other behaviors.
/// It validates the request using all registered IValidator&lt;TRequest&gt; validators.
/// If validation fails, throws ValidationException with all validation errors.
/// If no validators are registered, passes through to next behavior without validation.
/// </remarks>
/// <example>
/// <code>
/// // Define a validator
/// public class CreateProductCommandValidator : AbstractValidator&lt;CreateProductCommand&gt;
/// {
///     public CreateProductCommandValidator()
///     {
///         RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
///         RuleFor(x => x.Price).GreaterThan(0);
///     }
/// }
///
/// // Validation happens automatically in the pipeline
/// var command = new CreateProductCommand { Name = "", Price = -10 };
/// await _mediator.Send(command); // Throws ValidationException
/// </code>
/// </example>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of ValidationBehavior
    /// </summary>
    /// <param name="validators">Collection of validators for the request type</param>
    /// <param name="logger">Logger for validation failures</param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    /// <summary>
    /// Validates the request before passing to the next pipeline behavior
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="next">The next behavior in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the next behavior</returns>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count > 0)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning("Validation failed for {RequestName}: {Errors}",
                requestName,
                string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

            throw new ValidationException(failures);
        }

        return await next();
    }
}
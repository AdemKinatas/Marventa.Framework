using System.Collections.Generic;
using System.Linq;

namespace Marventa.Framework.Domain.Validators;

public abstract class BaseValidator<T>
{
    public ValidationResult Validate(T instance)
    {
        var errors = new List<ValidationError>();
        ValidateCore(instance, errors);
        return new ValidationResult(errors);
    }

    protected abstract void ValidateCore(T instance, List<ValidationError> errors);

    protected void AddError(List<ValidationError> errors, string propertyName, string errorMessage)
    {
        errors.Add(new ValidationError(propertyName, errorMessage));
    }
}

public class ValidationResult
{
    public bool IsValid => !Errors.Any();
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationResult(List<ValidationError> errors)
    {
        Errors = errors.AsReadOnly();
    }
}

public class ValidationError
{
    public string PropertyName { get; }
    public string ErrorMessage { get; }

    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
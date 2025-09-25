using FluentValidation;

namespace Marventa.Framework.Application.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected BaseValidator()
    {
        ConfigureValidation();
    }

    protected abstract void ConfigureValidation();

    protected void ValidateId(string propertyName = "Id")
    {
        RuleFor(x => GetPropertyValue(x, propertyName))
            .NotEmpty()
            .WithMessage($"{propertyName} is required")
            .Must(BeValidGuid)
            .WithMessage($"{propertyName} must be a valid GUID");
    }

    protected void ValidateEmail(string propertyName)
    {
        RuleFor(x => GetPropertyValue(x, propertyName))
            .EmailAddress()
            .WithMessage($"{propertyName} must be a valid email address")
            .When(x => !string.IsNullOrEmpty(GetPropertyValue(x, propertyName)));
    }

    protected void ValidateRequired(string propertyName, int maxLength = 0)
    {
        var rule = RuleFor(x => GetPropertyValue(x, propertyName))
            .NotEmpty()
            .WithMessage($"{propertyName} is required");

        if (maxLength > 0)
        {
            rule.MaximumLength(maxLength)
                .WithMessage($"{propertyName} must not exceed {maxLength} characters");
        }
    }

    protected void ValidateOptional(string propertyName, int maxLength = 0)
    {
        if (maxLength > 0)
        {
            RuleFor(x => GetPropertyValue(x, propertyName))
                .MaximumLength(maxLength)
                .WithMessage($"{propertyName} must not exceed {maxLength} characters")
                .When(x => !string.IsNullOrEmpty(GetPropertyValue(x, propertyName)));
        }
    }

    protected void ValidatePhoneNumber(string propertyName)
    {
        RuleFor(x => GetPropertyValue(x, propertyName))
            .Matches(@"^[\+]?[1-9][\d]{0,15}$")
            .WithMessage($"{propertyName} must be a valid phone number")
            .When(x => !string.IsNullOrEmpty(GetPropertyValue(x, propertyName)));
    }

    protected void ValidateUrl(string propertyName)
    {
        RuleFor(x => GetPropertyValue(x, propertyName))
            .Must(BeValidUrl)
            .WithMessage($"{propertyName} must be a valid URL")
            .When(x => !string.IsNullOrEmpty(GetPropertyValue(x, propertyName)));
    }

    protected void ValidatePrice(string propertyName)
    {
        RuleFor(x => GetDecimalPropertyValue(x, propertyName))
            .GreaterThanOrEqualTo(0)
            .WithMessage($"{propertyName} must be greater than or equal to 0")
            .ScalePrecision(2, 18)
            .WithMessage($"{propertyName} can have maximum 2 decimal places");
    }

    protected void ValidateQuantity(string propertyName)
    {
        RuleFor(x => GetIntPropertyValue(x, propertyName))
            .GreaterThanOrEqualTo(0)
            .WithMessage($"{propertyName} must be greater than or equal to 0");
    }

    protected void ValidateDateRange(string fromPropertyName, string toPropertyName)
    {
        RuleFor(x => GetDateTimePropertyValue(x, toPropertyName))
            .GreaterThan(x => GetDateTimePropertyValue(x, fromPropertyName))
            .WithMessage($"{toPropertyName} must be greater than {fromPropertyName}")
            .When(x => GetDateTimePropertyValue(x, fromPropertyName).HasValue && GetDateTimePropertyValue(x, toPropertyName).HasValue);
    }

    private static bool BeValidGuid(string? value)
    {
        return Guid.TryParse(value, out _);
    }

    private static bool BeValidUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    private static string? GetPropertyValue(T instance, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        return property?.GetValue(instance)?.ToString();
    }

    private static decimal GetDecimalPropertyValue(T instance, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        var value = property?.GetValue(instance);
        return value switch
        {
            decimal d => d,
            double db => (decimal)db,
            float f => (decimal)f,
            int i => i,
            long l => l,
            _ => 0
        };
    }

    private static int GetIntPropertyValue(T instance, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        var value = property?.GetValue(instance);
        return value switch
        {
            int i => i,
            decimal d => (int)d,
            double db => (int)db,
            float f => (int)f,
            long l => (int)l,
            _ => 0
        };
    }

    private static DateTime? GetDateTimePropertyValue(T instance, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        var value = property?.GetValue(instance);
        return value as DateTime?;
    }
}
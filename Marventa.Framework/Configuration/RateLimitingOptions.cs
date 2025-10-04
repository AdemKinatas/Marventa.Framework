using System.ComponentModel.DataAnnotations;

namespace Marventa.Framework.Configuration;

/// <summary>
/// Configuration options for rate limiting.
/// </summary>
public class RateLimitingOptions : IValidatableObject
{
    public const string SectionName = "RateLimiting";

    public int RequestLimit { get; set; } = 100;
    public int TimeWindowSeconds { get; set; } = 60;
    public RateLimitStrategy Strategy { get; set; } = RateLimitStrategy.IpAddress;
    public bool EnableGlobalRateLimit { get; set; } = true;
    public List<string> ExcludedPaths { get; set; } = new();
    public List<string> ExcludedIpAddresses { get; set; } = new();
    public string? CustomHeaderName { get; set; }
    public bool ReturnRateLimitHeaders { get; set; } = true;
    public string? CustomErrorMessage { get; set; }

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RequestLimit <= 0)
        {
            yield return new ValidationResult(
                "Rate limit RequestLimit must be greater than 0.",
                new[] { nameof(RequestLimit) });
        }

        if (TimeWindowSeconds <= 0)
        {
            yield return new ValidationResult(
                "Rate limit TimeWindowSeconds must be greater than 0.",
                new[] { nameof(TimeWindowSeconds) });
        }

        if (Strategy == RateLimitStrategy.CustomHeader && string.IsNullOrWhiteSpace(CustomHeaderName))
        {
            yield return new ValidationResult(
                "CustomHeaderName is required when using CustomHeader strategy.",
                new[] { nameof(CustomHeaderName) });
        }
    }
}

public enum RateLimitStrategy
{
    IpAddress,
    UserId,
    ApiKey,
    CustomHeader
}

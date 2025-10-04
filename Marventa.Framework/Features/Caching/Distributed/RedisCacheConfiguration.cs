using System.ComponentModel.DataAnnotations;

namespace Marventa.Framework.Features.Caching.Distributed;

/// <summary>
/// Configuration options for Redis cache.
/// </summary>
public class RedisCacheConfiguration : IValidatableObject
{
    /// <summary>
    /// Gets or sets the Redis connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Gets or sets the Redis instance name (prefix for all keys).
    /// </summary>
    public string InstanceName { get; set; } = "Marventa:";

    /// <summary>
    /// Gets or sets the default expiration time in minutes.
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            yield return new ValidationResult(
                "Redis ConnectionString cannot be null or empty.",
                new[] { nameof(ConnectionString) });
        }

        if (DefaultExpirationMinutes <= 0)
        {
            yield return new ValidationResult(
                "Redis DefaultExpirationMinutes must be greater than 0.",
                new[] { nameof(DefaultExpirationMinutes) });
        }
    }
}

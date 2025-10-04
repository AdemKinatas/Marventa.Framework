using System.ComponentModel.DataAnnotations;

namespace Marventa.Framework.Configuration;

/// <summary>
/// Configuration options for Swagger/OpenAPI documentation.
/// </summary>
public class SwaggerOptions : IValidatableObject
{
    public const string SectionName = "Swagger";

    public bool Enabled { get; set; } = true;
    public string Title { get; set; } = "API";
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "v1";
    public bool RequireAuthorization { get; set; } = true;
    public string[] EnvironmentRestriction { get; set; } = new[] { "Development" };
    public ContactInfo? Contact { get; set; }
    public LicenseInfo? License { get; set; }

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult(
                "Swagger Title cannot be null or empty.",
                new[] { nameof(Title) });
        }

        if (string.IsNullOrWhiteSpace(Version))
        {
            yield return new ValidationResult(
                "Swagger Version cannot be null or empty.",
                new[] { nameof(Version) });
        }
    }

    public class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class LicenseInfo
    {
        public string Name { get; set; } = "MIT";
        public string Url { get; set; } = string.Empty;
    }
}

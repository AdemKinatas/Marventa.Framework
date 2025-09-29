namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// Configuration options for security services
/// </summary>
public class SecurityOptions
{
    /// <summary>
    /// JWT secret key
    /// </summary>
    public string? JwtSecret { get; set; }

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string? JwtIssuer { get; set; }

    /// <summary>
    /// JWT audience
    /// </summary>
    public string? JwtAudience { get; set; }

    /// <summary>
    /// JWT expiration time in minutes
    /// </summary>
    public int JwtExpirationMinutes { get; set; } = 60;
}
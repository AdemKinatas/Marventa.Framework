namespace Marventa.Framework.Security.Authentication;

/// <summary>
/// Configuration options for JWT token generation and validation.
/// </summary>
public class JwtConfiguration
{
    /// <summary>
    /// Gets or sets the secret key used for signing JWT tokens.
    /// Must be at least 256 bits (32 characters) for HS256 algorithm.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issuer (iss claim) for JWT tokens.
    /// Typically the name of your application or authentication server.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience (aud claim) for JWT tokens.
    /// Typically the intended recipient of the token (e.g., your API).
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration time for access tokens in minutes.
    /// Default is 60 minutes (1 hour).
    /// Recommended: 15-60 minutes for security.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}

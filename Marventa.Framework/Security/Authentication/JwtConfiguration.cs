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

    /// <summary>
    /// Gets or sets the expiration time for refresh tokens in days.
    /// Default is 7 days.
    /// Recommended: 7-30 days depending on security requirements.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// Gets or sets whether to enable automatic token rotation on refresh.
    /// When enabled, each refresh creates a new refresh token and revokes the old one.
    /// Default is true for enhanced security.
    /// </summary>
    public bool EnableTokenRotation { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of concurrent refresh tokens per user.
    /// Set to 0 for unlimited. Default is 5.
    /// </summary>
    public int MaxRefreshTokensPerUser { get; set; } = 5;

    /// <summary>
    /// Gets or sets whether to include IP address validation in refresh tokens.
    /// When enabled, refresh tokens can only be used from the IP they were created with.
    /// Default is false to support mobile clients with changing IPs.
    /// </summary>
    public bool ValidateIpAddress { get; set; } = false;
}

namespace Marventa.Framework.Security.Authentication.Models;

/// <summary>
/// Represents a refresh token used for obtaining new access tokens without re-authentication.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the refresh token.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the actual token value.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier associated with this refresh token.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the token was revoked (if applicable).
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for token revocation (if applicable).
    /// </summary>
    public string? RevokedReason { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the token was created.
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the token was revoked.
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Gets or sets the token that replaced this one (if rotated).
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Determines whether the refresh token is currently active and usable.
    /// </summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Determines whether the refresh token has expired.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Determines whether the refresh token has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedAt != null;
}

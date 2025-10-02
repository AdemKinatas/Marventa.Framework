using Marventa.Framework.Security.Authentication.Models;
using System.Security.Claims;

namespace Marventa.Framework.Security.Authentication.Abstractions;

/// <summary>
/// Provides JWT token generation, validation, and management services.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token with the specified claims.
    /// </summary>
    /// <param name="claims">The claims to include in the token.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Generates a JWT access token for a specific user with custom claims.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="roles">The user's roles.</param>
    /// <param name="additionalClaims">Optional additional claims to include.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateAccessToken(
        string userId,
        string email,
        IEnumerable<string>? roles = null,
        IDictionary<string, string>? additionalClaims = null);

    /// <summary>
    /// Validates a JWT token and returns the claims principal if valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The claims principal if valid; otherwise, null.</returns>
    ClaimsPrincipal? ValidateAccessToken(string token);

    /// <summary>
    /// Extracts the user ID from a JWT token without full validation.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The user ID if found; otherwise, null.</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Extracts all claims from a JWT token without full validation.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The claims collection.</returns>
    IEnumerable<Claim> GetClaimsFromToken(string token);

    /// <summary>
    /// Gets the remaining lifetime of a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The time remaining until expiration, or null if expired or invalid.</returns>
    TimeSpan? GetTokenRemainingLifetime(string token);

    /// <summary>
    /// Checks if a JWT token is expired.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>True if the token is expired; otherwise, false.</returns>
    bool IsTokenExpired(string token);
}

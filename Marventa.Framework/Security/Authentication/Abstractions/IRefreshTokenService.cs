using Marventa.Framework.Security.Authentication.Models;

namespace Marventa.Framework.Security.Authentication.Abstractions;

/// <summary>
/// Provides refresh token generation, validation, rotation, and revocation services.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="ipAddress">The IP address of the client requesting the token.</param>
    /// <returns>The generated refresh token.</returns>
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string? ipAddress = null);

    /// <summary>
    /// Validates a refresh token and returns it if valid and active.
    /// </summary>
    /// <param name="token">The refresh token to validate.</param>
    /// <returns>The refresh token if valid and active; otherwise, null.</returns>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);

    /// <summary>
    /// Rotates a refresh token by revoking the old one and generating a new one.
    /// This is used during token refresh to maintain security.
    /// </summary>
    /// <param name="oldToken">The old refresh token to revoke.</param>
    /// <param name="ipAddress">The IP address of the client requesting the rotation.</param>
    /// <returns>The new refresh token.</returns>
    Task<RefreshToken> RotateRefreshTokenAsync(string oldToken, string? ipAddress = null);

    /// <summary>
    /// Revokes a refresh token, making it unusable.
    /// </summary>
    /// <param name="token">The refresh token to revoke.</param>
    /// <param name="ipAddress">The IP address of the client requesting revocation.</param>
    /// <param name="reason">The reason for revocation.</param>
    /// <returns>True if revoked successfully; otherwise, false.</returns>
    Task<bool> RevokeRefreshTokenAsync(string token, string? ipAddress = null, string? reason = null);

    /// <summary>
    /// Revokes all refresh tokens for a specific user.
    /// Useful for logout from all devices or security incidents.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="ipAddress">The IP address of the client requesting revocation.</param>
    /// <param name="reason">The reason for revocation.</param>
    /// <returns>The number of tokens revoked.</returns>
    Task<int> RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? reason = null);

    /// <summary>
    /// Gets all active refresh tokens for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of active refresh tokens.</returns>
    Task<IEnumerable<RefreshToken>> GetUserActiveTokensAsync(string userId);

    /// <summary>
    /// Gets all refresh tokens (active and revoked) for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of all refresh tokens.</returns>
    Task<IEnumerable<RefreshToken>> GetUserTokenHistoryAsync(string userId);

    /// <summary>
    /// Cleans up expired refresh tokens from storage.
    /// Should be called periodically (e.g., via a background job).
    /// </summary>
    /// <returns>The number of tokens cleaned up.</returns>
    Task<int> CleanupExpiredTokensAsync();

    /// <summary>
    /// Saves a refresh token to storage.
    /// </summary>
    /// <param name="refreshToken">The refresh token to save.</param>
    Task SaveRefreshTokenAsync(RefreshToken refreshToken);

    /// <summary>
    /// Updates an existing refresh token in storage.
    /// </summary>
    /// <param name="refreshToken">The refresh token to update.</param>
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
}

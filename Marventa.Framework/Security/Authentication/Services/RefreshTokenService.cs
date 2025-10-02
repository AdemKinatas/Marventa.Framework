using Marventa.Framework.Features.Caching.Abstractions;
using Marventa.Framework.Security.Authentication.Abstractions;
using Marventa.Framework.Security.Authentication.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Marventa.Framework.Security.Authentication.Services;

/// <summary>
/// Provides refresh token generation, validation, rotation, and revocation services.
/// Uses distributed cache for storage to support multi-server deployments.
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly ICacheService _cacheService;
    private readonly JwtConfiguration _configuration;
    private const string CacheKeyPrefix = "refresh_token:";
    private const string UserTokensKeyPrefix = "user_tokens:";

    public RefreshTokenService(
        ICacheService cacheService,
        IOptions<JwtConfiguration> configuration)
    {
        _cacheService = cacheService;
        _configuration = configuration.Value;
    }

    /// <inheritdoc/>
    public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string? ipAddress = null)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            Token = GenerateSecureToken(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_configuration.RefreshTokenExpirationDays),
            CreatedByIp = ipAddress
        };

        await SaveRefreshTokenAsync(token);
        await AddTokenToUserListAsync(userId, token.Token);

        return token;
    }

    /// <inheritdoc/>
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await GetRefreshTokenFromCacheAsync(token);

        if (refreshToken == null)
        {
            return null;
        }

        // Check if token is active (not revoked and not expired)
        if (!refreshToken.IsActive)
        {
            return null;
        }

        return refreshToken;
    }

    /// <inheritdoc/>
    public async Task<RefreshToken> RotateRefreshTokenAsync(string oldToken, string? ipAddress = null)
    {
        var oldRefreshToken = await GetRefreshTokenFromCacheAsync(oldToken);

        if (oldRefreshToken == null)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        if (!oldRefreshToken.IsActive)
        {
            throw new InvalidOperationException("Refresh token is not active.");
        }

        // Generate new token
        var newRefreshToken = await GenerateRefreshTokenAsync(oldRefreshToken.UserId, ipAddress);

        // Revoke old token and link to new one
        oldRefreshToken.RevokedAt = DateTime.UtcNow;
        oldRefreshToken.RevokedByIp = ipAddress;
        oldRefreshToken.ReplacedByToken = newRefreshToken.Token;
        oldRefreshToken.RevokedReason = "Token rotated during refresh";

        await UpdateRefreshTokenAsync(oldRefreshToken);

        return newRefreshToken;
    }

    /// <inheritdoc/>
    public async Task<bool> RevokeRefreshTokenAsync(string token, string? ipAddress = null, string? reason = null)
    {
        var refreshToken = await GetRefreshTokenFromCacheAsync(token);

        if (refreshToken == null || refreshToken.IsRevoked)
        {
            return false;
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.RevokedReason = reason ?? "Token manually revoked";

        await UpdateRefreshTokenAsync(refreshToken);
        await RemoveTokenFromUserListAsync(refreshToken.UserId, token);

        return true;
    }

    /// <inheritdoc/>
    public async Task<int> RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? reason = null)
    {
        var userTokens = await GetUserActiveTokensAsync(userId);
        var revokedCount = 0;

        foreach (var token in userTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.RevokedReason = reason ?? "All user tokens revoked";

            await UpdateRefreshTokenAsync(token);
            revokedCount++;
        }

        // Clear user token list
        await _cacheService.RemoveAsync(GetUserTokensKey(userId));

        return revokedCount;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetUserActiveTokensAsync(string userId)
    {
        var tokenList = await GetUserTokenListAsync(userId);
        var activeTokens = new List<RefreshToken>();

        foreach (var tokenString in tokenList)
        {
            var token = await GetRefreshTokenFromCacheAsync(tokenString);
            if (token != null && token.IsActive)
            {
                activeTokens.Add(token);
            }
        }

        return activeTokens;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetUserTokenHistoryAsync(string userId)
    {
        var tokenList = await GetUserTokenListAsync(userId);
        var allTokens = new List<RefreshToken>();

        foreach (var tokenString in tokenList)
        {
            var token = await GetRefreshTokenFromCacheAsync(tokenString);
            if (token != null)
            {
                allTokens.Add(token);
            }
        }

        return allTokens;
    }

    /// <inheritdoc/>
    public async Task<int> CleanupExpiredTokensAsync()
    {
        // Note: This is a simplified implementation.
        // In production, you might want to use a background job service
        // and iterate through a dedicated index or database.

        // For cache-based implementation, expired items are typically
        // automatically removed by the cache provider.
        // This method can be implemented if you maintain a separate index.

        await Task.CompletedTask;
        return 0;
    }

    /// <inheritdoc/>
    public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        var cacheKey = GetTokenCacheKey(refreshToken.Token);

        await _cacheService.SetAsync(
            cacheKey,
            refreshToken,
            new CacheOptions
            {
                AbsoluteExpiration = refreshToken.ExpiresAt - DateTime.UtcNow,
                SlidingExpiration = null
            });
    }

    /// <inheritdoc/>
    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        var cacheKey = GetTokenCacheKey(refreshToken.Token);

        await _cacheService.SetAsync(
            cacheKey,
            refreshToken,
            new CacheOptions
            {
                AbsoluteExpiration = refreshToken.ExpiresAt - DateTime.UtcNow,
                SlidingExpiration = null
            });
    }

    #region Private Helper Methods

    private async Task<RefreshToken?> GetRefreshTokenFromCacheAsync(string token)
    {
        var cacheKey = GetTokenCacheKey(token);
        return await _cacheService.GetAsync<RefreshToken>(cacheKey);
    }

    private async Task<List<string>> GetUserTokenListAsync(string userId)
    {
        var cacheKey = GetUserTokensKey(userId);
        var tokenList = await _cacheService.GetAsync<List<string>>(cacheKey);
        return tokenList ?? new List<string>();
    }

    private async Task AddTokenToUserListAsync(string userId, string token)
    {
        var tokenList = await GetUserTokenListAsync(userId);
        tokenList.Add(token);

        var cacheKey = GetUserTokensKey(userId);
        await _cacheService.SetAsync(
            cacheKey,
            tokenList,
            new CacheOptions
            {
                AbsoluteExpiration = TimeSpan.FromDays(_configuration.RefreshTokenExpirationDays + 1)
            });
    }

    private async Task RemoveTokenFromUserListAsync(string userId, string token)
    {
        var tokenList = await GetUserTokenListAsync(userId);
        tokenList.Remove(token);

        var cacheKey = GetUserTokensKey(userId);
        if (tokenList.Any())
        {
            await _cacheService.SetAsync(
                cacheKey,
                tokenList,
                new CacheOptions
                {
                    AbsoluteExpiration = TimeSpan.FromDays(_configuration.RefreshTokenExpirationDays + 1)
                });
        }
        else
        {
            await _cacheService.RemoveAsync(cacheKey);
        }
    }

    private static string GetTokenCacheKey(string token) => $"{CacheKeyPrefix}{token}";

    private static string GetUserTokensKey(string userId) => $"{UserTokensKeyPrefix}{userId}";

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    #endregion
}

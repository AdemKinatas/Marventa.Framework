using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Security;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
    Task<string> GenerateRefreshTokenAsync(CancellationToken cancellationToken = default);
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default);
}

public class TokenInfo
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
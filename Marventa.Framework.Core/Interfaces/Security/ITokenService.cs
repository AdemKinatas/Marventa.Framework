using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces.Security;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
    Task<string> GenerateRefreshTokenAsync(CancellationToken cancellationToken = default);
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default);
}
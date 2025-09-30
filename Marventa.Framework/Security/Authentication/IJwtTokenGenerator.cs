using System.Security.Claims;

namespace Marventa.Framework.Security.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(IEnumerable<Claim> claims);
    ClaimsPrincipal? ValidateToken(string token);
}

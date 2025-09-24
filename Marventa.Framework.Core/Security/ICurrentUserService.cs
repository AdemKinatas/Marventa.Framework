using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Marventa.Framework.Core.Security;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<Claim> Claims { get; }
    bool IsInRole(string role);
    bool HasClaim(string claimType, string claimValue);
    T? GetClaimValue<T>(string claimType);
}
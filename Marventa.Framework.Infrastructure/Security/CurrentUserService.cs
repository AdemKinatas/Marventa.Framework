using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Marventa.Framework.Core.Security;
using Microsoft.AspNetCore.Http;

namespace Marventa.Framework.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles =>
        _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();

    public IEnumerable<Claim> Claims =>
        _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public bool HasClaim(string claimType, string claimValue)
    {
        return _httpContextAccessor.HttpContext?.User?.HasClaim(claimType, claimValue) ?? false;
    }

    public T? GetClaimValue<T>(string claimType)
    {
        var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
        if (string.IsNullOrEmpty(claimValue))
            return default;

        try
        {
            return (T)Convert.ChangeType(claimValue, typeof(T));
        }
        catch
        {
            return default;
        }
    }
}
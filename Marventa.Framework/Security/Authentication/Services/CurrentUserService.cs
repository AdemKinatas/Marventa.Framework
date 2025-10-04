using Marventa.Framework.Security.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Marventa.Framework.Security.Authentication.Services;

/// <summary>
/// Implementation of ICurrentUserService that retrieves user information from HTTP context claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc/>
    public Guid? UserId
    {
        get
        {
            var userIdClaim = GetClaimValue(ClaimTypes.NameIdentifier) ?? GetClaimValue("sub");
            return userIdClaim != null && Guid.TryParse(userIdClaim, out var userId)
                ? userId
                : null;
        }
    }

    /// <inheritdoc/>
    public string? Email => GetClaimValue(ClaimTypes.Email) ?? GetClaimValue("email");

    /// <inheritdoc/>
    public string? UserName => GetClaimValue(ClaimTypes.Name) ?? GetClaimValue("name") ?? GetClaimValue("preferred_username");

    /// <inheritdoc/>
    public Guid? TenantId
    {
        get
        {
            var tenantIdClaim = GetClaimValue("tenant_id") ?? GetClaimValue("tenantId");
            return tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId)
                ? tenantId
                : null;
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public IEnumerable<string> Roles
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return Enumerable.Empty<string>();

            return user.FindAll(ClaimTypes.Role)
                .Concat(user.FindAll("role"))
                .Select(c => c.Value)
                .Distinct()
                .ToList();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Permissions
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return Enumerable.Empty<string>();

            return user.FindAll("permission")
                .Concat(user.FindAll("permissions"))
                .Select(c => c.Value)
                .Distinct()
                .ToList();
        }
    }

    /// <inheritdoc/>
    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return false;

        return Permissions.Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a claim value from the current user's claims.
    /// </summary>
    /// <param name="claimType">The claim type to retrieve.</param>
    /// <returns>The claim value if found, otherwise null.</returns>
    private string? GetClaimValue(string claimType)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(claimType)?.Value;
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Authorization;

public class TenantAuthorizationService : ITenantAuthorization
{
    private readonly ITenantPermissionService _permissionService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TenantAuthorizationService> _logger;

    public TenantAuthorizationService(
        ITenantPermissionService permissionService,
        IMemoryCache cache,
        ILogger<TenantAuthorizationService> logger)
    {
        _permissionService = permissionService;
        _cache = cache;
        _logger = logger;
    }

    public bool CanAccessTenant(string userId, string tenantId)
    {
        return CanAccessTenantAsync(userId, tenantId).GetAwaiter().GetResult();
    }

    public bool HasPermission(string userId, string tenantId, string permission)
    {
        return HasPermissionAsync(userId, tenantId, permission).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessTenantAsync(string userId, string tenantId)
    {
        try
        {
            var cacheKey = $"tenant_access_{tenantId}_{userId}";

            if (_cache.TryGetValue<bool>(cacheKey, out var cached))
                return cached;

            var users = await _permissionService.GetTenantUsersAsync(tenantId);
            var hasAccess = users.Contains(userId);

            _cache.Set(cacheKey, hasAccess, TimeSpan.FromMinutes(5));

            return hasAccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking tenant access for user {UserId} in tenant {TenantId}", userId, tenantId);
            return false;
        }
    }

    public async Task<bool> HasPermissionAsync(string userId, string tenantId, string permission)
    {
        try
        {
            if (!await CanAccessTenantAsync(userId, tenantId))
                return false;

            var cacheKey = $"tenant_perm_{tenantId}_{userId}_{permission}";

            if (_cache.TryGetValue<bool>(cacheKey, out var cached))
                return cached;

            var permissions = await _permissionService.GetUserPermissionsAsync(userId, tenantId);
            var hasPermission = permissions.Contains(permission) || permissions.Contains(TenantPermissions.Admin);

            _cache.Set(cacheKey, hasPermission, TimeSpan.FromMinutes(5));

            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Permission} for user {UserId} in tenant {TenantId}", permission, userId, tenantId);
            return false;
        }
    }
}
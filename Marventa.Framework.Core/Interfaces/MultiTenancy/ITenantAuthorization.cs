namespace Marventa.Framework.Core.Interfaces.MultiTenancy;

public interface ITenantAuthorization
{
    bool CanAccessTenant(string userId, string tenantId);
    bool HasPermission(string userId, string tenantId, string permission);
    Task<bool> CanAccessTenantAsync(string userId, string tenantId);
    Task<bool> HasPermissionAsync(string userId, string tenantId, string permission);
}
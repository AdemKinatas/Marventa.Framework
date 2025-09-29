namespace Marventa.Framework.Core.Interfaces.MultiTenancy;

public interface ITenantPermissionService
{
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, string tenantId);
    Task<bool> GrantPermissionAsync(string userId, string tenantId, string permission);
    Task<bool> RevokePermissionAsync(string userId, string tenantId, string permission);
    Task<IEnumerable<string>> GetTenantUsersAsync(string tenantId);
}
namespace Marventa.Framework.Core.Interfaces;

public interface ITenantAuthorization
{
    bool CanAccessTenant(string userId, string tenantId);
    bool HasPermission(string userId, string tenantId, string permission);
    Task<bool> CanAccessTenantAsync(string userId, string tenantId);
    Task<bool> HasPermissionAsync(string userId, string tenantId, string permission);
}

public interface ITenantPermissionService
{
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, string tenantId);
    Task<bool> GrantPermissionAsync(string userId, string tenantId, string permission);
    Task<bool> RevokePermissionAsync(string userId, string tenantId, string permission);
    Task<IEnumerable<string>> GetTenantUsersAsync(string tenantId);
}

public static class TenantPermissions
{
    public const string Read = "tenant:read";
    public const string Write = "tenant:write";
    public const string Delete = "tenant:delete";
    public const string Admin = "tenant:admin";
    public const string ManageUsers = "tenant:users:manage";
    public const string ManageSettings = "tenant:settings:manage";
    public const string ViewReports = "tenant:reports:view";
    public const string ManageBilling = "tenant:billing:manage";
}

public interface ITenantScopedService<T>
{
    Task<T?> GetByTenantAsync(string tenantId, string id);
    Task<IEnumerable<T>> GetAllByTenantAsync(string tenantId);
    Task<T> CreateForTenantAsync(string tenantId, T entity);
    Task<T> UpdateForTenantAsync(string tenantId, T entity);
    Task DeleteForTenantAsync(string tenantId, string id);
}
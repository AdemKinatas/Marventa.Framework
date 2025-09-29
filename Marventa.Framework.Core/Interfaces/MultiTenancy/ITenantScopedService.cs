namespace Marventa.Framework.Core.Interfaces.MultiTenancy;

public interface ITenantScopedService<T>
{
    Task<T?> GetByTenantAsync(string tenantId, string id);
    Task<IEnumerable<T>> GetAllByTenantAsync(string tenantId);
    Task<T> CreateForTenantAsync(string tenantId, T entity);
    Task<T> UpdateForTenantAsync(string tenantId, T entity);
    Task DeleteForTenantAsync(string tenantId, string id);
}
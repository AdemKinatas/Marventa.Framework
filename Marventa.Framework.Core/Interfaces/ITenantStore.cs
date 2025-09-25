namespace Marventa.Framework.Core.Interfaces;

public interface ITenantStore
{
    Task<ITenant?> GetByIdAsync(string tenantId);
    Task<ITenant?> GetByHostAsync(string host);
    Task<ITenant?> GetByIdentifierAsync(string identifier);
    Task<IEnumerable<ITenant>> GetAllAsync();
    Task<ITenant> CreateAsync(ITenant tenant);
    Task<ITenant> UpdateAsync(ITenant tenant);
    Task DeleteAsync(string tenantId);
}
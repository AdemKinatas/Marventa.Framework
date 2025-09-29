namespace Marventa.Framework.Core.Interfaces.Caching;

public interface ITenantScopedCache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task ClearTenantCacheAsync(string tenantId, CancellationToken cancellationToken = default);
}
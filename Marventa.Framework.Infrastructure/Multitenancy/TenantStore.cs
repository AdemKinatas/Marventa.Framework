using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Multitenancy;

public class TenantStore : ITenantStore
{
    private readonly DbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<TenantStore> _logger;
    private const string CachePrefix = "tenant:";

    public TenantStore(DbContext context, ICacheService cache, ILogger<TenantStore> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ITenant?> GetByIdAsync(string tenantId)
    {
        var cacheKey = $"{CachePrefix}id:{tenantId}";
        var cachedTenant = await _cache.GetAsync<Tenant>(cacheKey);

        if (cachedTenant != null)
            return cachedTenant;

        var tenant = await _context.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.Id.ToString() == tenantId && t.IsActive);

        if (tenant != null)
        {
            await _cache.SetAsync(cacheKey, tenant, TimeSpan.FromMinutes(30));
        }

        return tenant;
    }

    public async Task<ITenant?> GetByHostAsync(string host)
    {
        var cacheKey = $"{CachePrefix}host:{host}";
        var cachedTenant = await _cache.GetAsync<Tenant>(cacheKey);

        if (cachedTenant != null)
            return cachedTenant;

        var tenant = await _context.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.Host == host && t.IsActive);

        if (tenant != null)
        {
            await _cache.SetAsync(cacheKey, tenant, TimeSpan.FromMinutes(30));
        }

        return tenant;
    }

    public async Task<ITenant?> GetByIdentifierAsync(string identifier)
    {
        return await GetByIdAsync(identifier);
    }

    public async Task<IEnumerable<ITenant>> GetAllAsync()
    {
        return await _context.Set<Tenant>()
            .Where(t => t.IsActive)
            .ToListAsync();
    }

    public async Task<ITenant> CreateAsync(ITenant tenant)
    {
        var entityTenant = new Tenant
        {
            Id = Guid.Parse(tenant.Id),
            Name = tenant.Name,
            ConnectionString = tenant.ConnectionString,
            Properties = tenant.Properties,
            IsActive = true
        };

        _context.Set<Tenant>().Add(entityTenant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tenant created: {TenantId}", tenant.Id);

        // Clear cache
        await InvalidateCacheAsync(tenant.Id);

        return entityTenant;
    }

    public async Task<ITenant> UpdateAsync(ITenant tenant)
    {
        var existing = await _context.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.Id.ToString() == tenant.Id);

        if (existing == null)
            throw new InvalidOperationException($"Tenant {tenant.Id} not found");

        existing.Name = tenant.Name;
        existing.ConnectionString = tenant.ConnectionString;
        existing.Properties = tenant.Properties;
        existing.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Tenant updated: {TenantId}", tenant.Id);

        // Clear cache
        await InvalidateCacheAsync(tenant.Id);

        return existing;
    }

    public async Task DeleteAsync(string tenantId)
    {
        var tenant = await _context.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.Id.ToString() == tenantId);

        if (tenant == null)
            return;

        tenant.IsActive = false;
        tenant.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Tenant deactivated: {TenantId}", tenantId);

        // Clear cache
        await InvalidateCacheAsync(tenantId);
    }

    private async Task InvalidateCacheAsync(string tenantId)
    {
        await _cache.RemoveAsync($"{CachePrefix}id:{tenantId}");
        // Additional cache keys could be invalidated here
    }
}
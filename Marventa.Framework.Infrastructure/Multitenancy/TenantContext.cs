using Marventa.Framework.Core.Interfaces.MultiTenancy;

namespace Marventa.Framework.Infrastructure.Multitenancy;

public class TenantContext : ITenantContext
{
    private ITenant? _tenant;

    public ITenant? CurrentTenant => _tenant;
    public string? TenantId => _tenant?.Id;
    public bool HasTenant => _tenant != null;

    public void SetTenant(ITenant tenant)
    {
        _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
    }

    public void SetTenant(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

        _tenant = new SimpleTenant(tenantId);
    }

    public void Clear()
    {
        _tenant = null;
    }

    private class SimpleTenant : ITenant
    {
        public SimpleTenant(string id)
        {
            Id = id;
            Name = id;
            Properties = new Dictionary<string, object>();
        }

        public string Id { get; }
        public string Name { get; }
        public string? ConnectionString => null;
        public Dictionary<string, object> Properties { get; }
    }
}
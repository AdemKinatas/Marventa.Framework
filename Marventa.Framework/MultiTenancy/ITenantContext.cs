namespace Marventa.Framework.MultiTenancy;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantName { get; }
    void SetTenant(Guid tenantId, string tenantName);
}

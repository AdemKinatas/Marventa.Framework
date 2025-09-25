namespace Marventa.Framework.Core.Interfaces;

public interface ITenantContext
{
    ITenant? CurrentTenant { get; }
    string? TenantId { get; }
    bool HasTenant { get; }
    void SetTenant(ITenant tenant);
    void SetTenant(string tenantId);
    void Clear();
}
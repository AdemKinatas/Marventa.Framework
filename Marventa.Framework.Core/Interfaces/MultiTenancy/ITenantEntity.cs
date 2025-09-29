namespace Marventa.Framework.Core.Interfaces.MultiTenancy;

public interface ITenantEntity
{
    string? TenantId { get; set; }
}
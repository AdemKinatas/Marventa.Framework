using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Core.Entities;

public abstract class TenantBaseEntity : BaseEntity, ITenantEntity
{
    public string? TenantId { get; set; }
}
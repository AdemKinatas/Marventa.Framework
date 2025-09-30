using Marventa.Framework.Core.Interfaces.MultiTenancy;

namespace Marventa.Framework.Core.Entities;

/// <summary>
/// Multi-tenant entity base class extending <see cref="BaseEntity"/> with tenant isolation.
/// Implements <see cref="ITenantEntity"/> for automatic tenant filtering in queries.
/// All queries will be automatically filtered by the current tenant context.
/// </summary>
public abstract class TenantBaseEntity : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the tenant identifier for this entity.
    /// Entities are automatically isolated by tenant in queries through global query filters.
    /// Null indicates a system-level entity not tied to a specific tenant.
    /// </summary>
    public string? TenantId { get; set; }
}
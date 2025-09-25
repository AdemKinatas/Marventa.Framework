using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Entities;

namespace Marventa.Framework.Infrastructure.Entities;

public class Tenant : BaseEntity, ITenant
{
    public string Name { get; set; } = string.Empty;
    public string? ConnectionString { get; set; }
    public string Host { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Dictionary<string, object> Properties { get; set; } = new();

    // Explicit interface implementation for string Id
    string ITenant.Id => base.Id.ToString();

    // Navigation properties for tenant-specific entities
    public virtual ICollection<TenantUser> Users { get; set; } = new List<TenantUser>();
}

public class TenantUser : BaseEntity
{
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public virtual Tenant Tenant { get; set; } = null!;
}
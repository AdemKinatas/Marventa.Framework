namespace Marventa.Framework.Infrastructure.Caching;

public class TenantCacheOptions
{
    public const string SectionName = "TenantCache";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableCompression { get; set; } = true;
    public long MaxSizePerTenant { get; set; } = 100 * 1024 * 1024; // 100MB per tenant
}
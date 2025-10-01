namespace Marventa.Framework.Configuration;

public class MultiTenancyOptions
{
    public const string SectionName = "MultiTenancy";

    public TenantResolutionStrategy Strategy { get; set; } = TenantResolutionStrategy.Header;
    public string HeaderName { get; set; } = "X-Tenant-Id";
    public string QueryParameterName { get; set; } = "tenantId";
    public string SubdomainPosition { get; set; } = "prefix";
    public bool RequireTenant { get; set; } = true;
    public string? DefaultTenantId { get; set; }
    public List<string> ExcludedPaths { get; set; } = new();
}

public enum TenantResolutionStrategy
{
    Header,
    QueryParameter,
    Subdomain,
    Claim,
    Custom
}

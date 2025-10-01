namespace Marventa.Framework.Configuration;

public class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public int RequestLimit { get; set; } = 100;
    public int TimeWindowSeconds { get; set; } = 60;
    public RateLimitStrategy Strategy { get; set; } = RateLimitStrategy.IpAddress;
    public bool EnableGlobalRateLimit { get; set; } = true;
    public List<string> ExcludedPaths { get; set; } = new();
    public List<string> ExcludedIpAddresses { get; set; } = new();
    public string? CustomHeaderName { get; set; }
    public bool ReturnRateLimitHeaders { get; set; } = true;
    public string? CustomErrorMessage { get; set; }
}

public enum RateLimitStrategy
{
    IpAddress,
    UserId,
    ApiKey,
    CustomHeader
}

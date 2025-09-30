namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Caching rule configuration
/// </summary>
public class CachingRule
{
    /// <summary>
    /// URL pattern to match (supports wildcards)
    /// </summary>
    public string Pattern { get; set; } = string.Empty;

    /// <summary>
    /// Content types to apply rule to
    /// </summary>
    public string[] ContentTypes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Cache TTL in seconds
    /// </summary>
    public int CacheTTLSeconds { get; set; }

    /// <summary>
    /// Browser cache TTL in seconds
    /// </summary>
    public int BrowserCacheTTLSeconds { get; set; }

    /// <summary>
    /// Whether to enable compression
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Custom cache headers
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Priority level (1-10, higher = more important)
    /// </summary>
    public int Priority { get; set; } = 5;
}

/// <summary>
/// Result of CDN configuration operation
/// </summary>
public class CDNConfigurationResult
{
    public string ConfigurationId { get; set; } = string.Empty;

    /// <summary>
    /// Rules that were successfully applied
    /// </summary>
    public CachingRule[] AppliedRules { get; set; } = Array.Empty<CachingRule>();

    /// <summary>
    /// Rules that failed to apply with reasons
    /// </summary>
    public Dictionary<CachingRule, string> FailedRules { get; set; } = new();

    public DateTime DeployedAt { get; set; }

    /// <summary>
    /// Estimated propagation time
    /// </summary>
    public TimeSpan EstimatedPropagationTime { get; set; }

    public bool Success { get; set; }
}
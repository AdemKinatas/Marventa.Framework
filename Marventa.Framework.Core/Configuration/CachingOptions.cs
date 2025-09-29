namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// Configuration options for caching services
/// </summary>
public class CachingOptions
{
    /// <summary>
    /// Cache provider name (Memory or Redis)
    /// </summary>
    public string Provider { get; set; } = "Memory";

    /// <summary>
    /// Connection string for cache provider
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Default expiration time in minutes
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;
}
namespace Marventa.Framework.Configuration;

/// <summary>
/// Configuration options for in-memory caching.
/// </summary>
public class MemoryCacheConfiguration
{
    public const string SectionName = "MemoryCache";

    /// <summary>
    /// Gets or sets the maximum number of cache entries.
    /// Default is 1024.
    /// </summary>
    public long SizeLimit { get; set; } = 1024;

    /// <summary>
    /// Gets or sets the percentage of entries to remove when the size limit is exceeded.
    /// Value should be between 0 and 1. Default is 0.25 (25%).
    /// </summary>
    public double CompactionPercentage { get; set; } = 0.25;

    /// <summary>
    /// Gets or sets the minimum time interval between compaction scans.
    /// Default is 1 minute.
    /// </summary>
    public TimeSpan? ExpirationScanFrequency { get; set; } = TimeSpan.FromMinutes(1);
}

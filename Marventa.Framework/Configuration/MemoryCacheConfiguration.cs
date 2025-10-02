namespace Marventa.Framework.Configuration;

/// <summary>
/// Configuration options for in-memory caching.
/// </summary>
public class MemoryCacheConfiguration
{
    public const string SectionName = "MemoryCache";

    /// <summary>
    /// Gets or sets the maximum number of cache entries.
    /// If not set, no size limit is enforced.
    /// </summary>
    public long? SizeLimit { get; set; }

    /// <summary>
    /// Gets or sets the percentage of entries to remove when the size limit is exceeded.
    /// Value should be between 0 and 1. Only applies if SizeLimit is set.
    /// </summary>
    public double? CompactionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the minimum time interval between compaction scans.
    /// If not set, uses default .NET behavior.
    /// </summary>
    public TimeSpan? ExpirationScanFrequency { get; set; }
}

namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// CDN performance and usage metrics
/// </summary>
public class CDNMetrics
{
    /// <summary>
    /// Time range for these metrics
    /// </summary>
    public TimeRange TimeRange { get; set; } = new();

    /// <summary>
    /// Total requests served
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// Total bandwidth consumed in bytes
    /// </summary>
    public long TotalBandwidthBytes { get; set; }

    /// <summary>
    /// Cache hit ratio (0.0 to 1.0)
    /// </summary>
    public double CacheHitRatio { get; set; }

    /// <summary>
    /// Average response time in milliseconds
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Requests by geographic region
    /// </summary>
    public Dictionary<string, long> RequestsByRegion { get; set; } = new();

    /// <summary>
    /// Bandwidth by geographic region
    /// </summary>
    public Dictionary<string, long> BandwidthByRegion { get; set; } = new();

    /// <summary>
    /// Status code distribution
    /// </summary>
    public Dictionary<int, long> StatusCodes { get; set; } = new();

    /// <summary>
    /// Top referrers
    /// </summary>
    public Dictionary<string, long> TopReferrers { get; set; } = new();

    /// <summary>
    /// Peak requests per second
    /// </summary>
    public double PeakRequestsPerSecond { get; set; }

    /// <summary>
    /// Error rate percentage
    /// </summary>
    public double ErrorRatePercentage { get; set; }
}

/// <summary>
/// Time range for metrics
/// </summary>
public class TimeRange
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
}
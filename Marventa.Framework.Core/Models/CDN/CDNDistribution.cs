namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// CDN distribution status across edge locations
/// </summary>
public class CDNDistributionStatus
{
    /// <summary>
    /// File identifier
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Overall distribution status
    /// </summary>
    public DistributionState Status { get; set; }

    /// <summary>
    /// Status by edge location
    /// </summary>
    public Dictionary<string, EdgeLocationStatus> EdgeStatuses { get; set; } = new();

    /// <summary>
    /// Total edge locations
    /// </summary>
    public int TotalEdgeLocations { get; set; }

    /// <summary>
    /// Edge locations with content
    /// </summary>
    public int DistributedEdgeLocations { get; set; }

    /// <summary>
    /// Distribution percentage (0.0 to 1.0)
    /// </summary>
    public double DistributionPercentage => TotalEdgeLocations > 0 ? (double)DistributedEdgeLocations / TotalEdgeLocations : 0;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Status of edge location
/// </summary>
public class EdgeLocationStatus
{
    /// <summary>
    /// Edge location identifier
    /// </summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>
    /// Geographic region
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public EdgeStatus Status { get; set; }

    /// <summary>
    /// Last successful sync time
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// Cache hit ratio at this edge
    /// </summary>
    public double CacheHitRatio { get; set; }
}
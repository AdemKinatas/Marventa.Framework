namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Result of cache invalidation operation
/// </summary>
public class CDNInvalidationResult
{
    /// <summary>
    /// Invalidation request ID
    /// </summary>
    public string InvalidationId { get; set; } = string.Empty;

    /// <summary>
    /// Status of each URL invalidation
    /// </summary>
    public Dictionary<string, InvalidationStatus> UrlStatuses { get; set; } = new();

    /// <summary>
    /// Estimated completion time
    /// </summary>
    public DateTime EstimatedCompletionTime { get; set; }

    /// <summary>
    /// Number of edge locations affected
    /// </summary>
    public int EdgeLocationsCount { get; set; }

    /// <summary>
    /// Overall success status
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Result of cache warming operation
/// </summary>
public class CDNWarmupResult
{
    /// <summary>
    /// Warmup request ID
    /// </summary>
    public string WarmupId { get; set; } = string.Empty;

    /// <summary>
    /// Status of each URL warmup
    /// </summary>
    public Dictionary<string, WarmupStatus> UrlStatuses { get; set; } = new();

    /// <summary>
    /// Edge locations that will be warmed
    /// </summary>
    public string[] EdgeLocations { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Estimated completion time
    /// </summary>
    public DateTime EstimatedCompletionTime { get; set; }

    /// <summary>
    /// Overall success status
    /// </summary>
    public bool Success { get; set; }
}
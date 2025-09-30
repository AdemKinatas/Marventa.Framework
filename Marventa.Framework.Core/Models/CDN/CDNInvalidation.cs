namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Result of cache invalidation operation
/// </summary>
public class CDNInvalidationResult
{
    public string InvalidationId { get; set; } = string.Empty;

    /// <summary>
    /// Status of each URL invalidation
    /// </summary>
    public Dictionary<string, InvalidationStatus> UrlStatuses { get; set; } = new();

    public DateTime EstimatedCompletionTime { get; set; }

    /// <summary>
    /// Number of edge locations affected
    /// </summary>
    public int EdgeLocationsCount { get; set; }

    public bool Success { get; set; }
}

/// <summary>
/// Result of cache warming operation
/// </summary>
public class CDNWarmupResult
{
    public string WarmupId { get; set; } = string.Empty;

    /// <summary>
    /// Status of each URL warmup
    /// </summary>
    public Dictionary<string, WarmupStatus> UrlStatuses { get; set; } = new();

    /// <summary>
    /// Edge locations that will be warmed
    /// </summary>
    public string[] EdgeLocations { get; set; } = Array.Empty<string>();

    public DateTime EstimatedCompletionTime { get; set; }
    public bool Success { get; set; }
}
namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Options for CDN upload operations
/// </summary>
public class CDNUploadOptions
{
    public string? CacheControl { get; set; }

    /// <summary>
    /// Custom headers to include
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Geographic regions to prioritize for distribution
    /// </summary>
    public string[] PriorityRegions { get; set; } = Array.Empty<string>();

    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Custom metadata tags
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new();

    /// <summary>
    /// Access level for the content
    /// </summary>
    public CDNAccessLevel AccessLevel { get; set; } = CDNAccessLevel.Public;
}

/// <summary>
/// Result of CDN upload operation
/// </summary>
public class CDNUploadResult
{
    public string CDNUrl { get; set; } = string.Empty;

    /// <summary>
    /// Regional URLs for optimized access
    /// </summary>
    public Dictionary<string, string> RegionalUrls { get; set; } = new();

    public string CDNFileId { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// ETag for caching
    /// </summary>
    public string? ETag { get; set; }

    /// <summary>
    /// Estimated propagation time to all edges
    /// </summary>
    public TimeSpan EstimatedPropagationTime { get; set; }

    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
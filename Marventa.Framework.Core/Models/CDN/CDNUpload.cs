namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Options for CDN upload operations
/// </summary>
public class CDNUploadOptions
{
    /// <summary>
    /// Cache-Control header value
    /// </summary>
    public string? CacheControl { get; set; }

    /// <summary>
    /// Custom headers to include
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Geographic regions to prioritize for distribution
    /// </summary>
    public string[] PriorityRegions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to enable compression
    /// </summary>
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
    /// <summary>
    /// Primary CDN URL
    /// </summary>
    public string CDNUrl { get; set; } = string.Empty;

    /// <summary>
    /// Regional URLs for optimized access
    /// </summary>
    public Dictionary<string, string> RegionalUrls { get; set; } = new();

    /// <summary>
    /// File identifier in CDN
    /// </summary>
    public string CDNFileId { get; set; } = string.Empty;

    /// <summary>
    /// Upload timestamp
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// File size in CDN
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// ETag for caching
    /// </summary>
    public string? ETag { get; set; }

    /// <summary>
    /// Estimated propagation time to all edges
    /// </summary>
    public TimeSpan EstimatedPropagationTime { get; set; }

    /// <summary>
    /// Whether upload was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if upload failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Result of thumbnail generation operation
/// </summary>
public class ThumbnailResult
{
    /// <summary>
    /// Generated thumbnails by size name
    /// </summary>
    public Dictionary<string, ThumbnailInfo> Thumbnails { get; set; } = new();

    /// <summary>
    /// Original image dimensions
    /// </summary>
    public ImageDimensions OriginalDimensions { get; set; } = new();

    /// <summary>
    /// Total processing time
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether all thumbnails were generated successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Any errors during generation
    /// </summary>
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Information about a generated thumbnail
/// </summary>
public class ThumbnailInfo
{
    /// <summary>
    /// Thumbnail image stream
    /// </summary>
    public Stream ImageStream { get; set; } = Stream.Null;

    /// <summary>
    /// Thumbnail dimensions
    /// </summary>
    public ImageDimensions Dimensions { get; set; } = new();

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Image format
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Thumbnail size descriptor
    /// </summary>
    public string Size { get; set; } = string.Empty;

    /// <summary>
    /// Quality setting used
    /// </summary>
    public int Quality { get; set; }
}

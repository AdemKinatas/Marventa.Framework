namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Result of watermark application
/// </summary>
public class WatermarkResult
{
    /// <summary>
    /// Image with applied watermark
    /// </summary>
    public Stream WatermarkedImage { get; set; } = Stream.Null;

    /// <summary>
    /// Final image dimensions
    /// </summary>
    public ImageDimensions Dimensions { get; set; } = new();

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Watermark position used
    /// </summary>
    public WatermarkPosition Position { get; set; }

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether watermark was successfully applied
    /// </summary>
    public bool WatermarkApplied { get; set; }

    /// <summary>
    /// Whether watermarking was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Result of image processing operation
/// </summary>
public class ProcessingResult
{
    /// <summary>
    /// Processed image stream
    /// </summary>
    public Stream ProcessedImage { get; set; } = Stream.Null;

    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Final image dimensions
    /// </summary>
    public ImageDimensions Dimensions { get; set; } = new();

    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Processing metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    public long ProcessingTimeMs { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public long OriginalSize { get; set; }
    public long ProcessedSize { get; set; }

    /// <summary>
    /// Compression ratio (0.0 to 1.0)
    /// </summary>
    public double CompressionRatio { get; set; }

    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

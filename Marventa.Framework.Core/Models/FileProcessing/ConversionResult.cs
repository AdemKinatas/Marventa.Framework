namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Result of format conversion operation
/// </summary>
public class ConversionResult
{
    /// <summary>
    /// Converted image stream
    /// </summary>
    public Stream ConvertedImage { get; set; } = Stream.Null;

    /// <summary>
    /// Source format
    /// </summary>
    public string SourceFormat { get; set; } = string.Empty;

    /// <summary>
    /// Target format
    /// </summary>
    public string TargetFormat { get; set; } = string.Empty;

    /// <summary>
    /// Original file size
    /// </summary>
    public long OriginalSizeBytes { get; set; }

    /// <summary>
    /// Converted file size
    /// </summary>
    public long ConvertedSizeBytes { get; set; }

    /// <summary>
    /// Quality setting used
    /// </summary>
    public int Quality { get; set; }

    /// <summary>
    /// Image dimensions
    /// </summary>
    public ImageDimensions Dimensions { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether conversion was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if conversion failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

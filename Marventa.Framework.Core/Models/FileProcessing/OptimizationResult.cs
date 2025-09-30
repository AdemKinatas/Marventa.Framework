namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Result of image optimization operation
/// </summary>
public class OptimizationResult
{
    /// <summary>
    /// Optimized image stream
    /// </summary>
    public Stream OptimizedImage { get; set; } = Stream.Null;

    public long OriginalSizeBytes { get; set; }
    public long OptimizedSizeBytes { get; set; }

    /// <summary>
    /// Compression ratio (0.0 to 1.0)
    /// </summary>
    public double CompressionRatio => OriginalSizeBytes > 0 ? (double)OptimizedSizeBytes / OriginalSizeBytes : 1.0;

    /// <summary>
    /// Space saved in bytes
    /// </summary>
    public long SpaceSavedBytes => OriginalSizeBytes - OptimizedSizeBytes;

    /// <summary>
    /// Space saved as percentage
    /// </summary>
    public double SpaceSavedPercentage => OriginalSizeBytes > 0 ? (double)SpaceSavedBytes / OriginalSizeBytes * 100 : 0;

    public OptimizationLevel Level { get; set; }
    public long ProcessingTimeMs { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public bool Success { get; set; }

    /// <summary>
    /// Final image quality score (if available)
    /// </summary>
    public double? QualityScore { get; set; }
}

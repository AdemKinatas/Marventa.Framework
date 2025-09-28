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

    /// <summary>
    /// Output format
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Final image dimensions
    /// </summary>
    public ImageDimensions Dimensions { get; set; } = new();

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Processing metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Original file size in bytes
    /// </summary>
    public long OriginalSize { get; set; }

    /// <summary>
    /// Processed file size in bytes
    /// </summary>
    public long ProcessedSize { get; set; }

    /// <summary>
    /// Compression ratio (0.0 to 1.0)
    /// </summary>
    public double CompressionRatio { get; set; }

    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

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

/// <summary>
/// Result of image optimization operation
/// </summary>
public class OptimizationResult
{
    /// <summary>
    /// Optimized image stream
    /// </summary>
    public Stream OptimizedImage { get; set; } = Stream.Null;

    /// <summary>
    /// Original file size in bytes
    /// </summary>
    public long OriginalSizeBytes { get; set; }

    /// <summary>
    /// Optimized file size in bytes
    /// </summary>
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

    /// <summary>
    /// Optimization level used
    /// </summary>
    public OptimizationLevel Level { get; set; }

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether optimization was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Final image quality score (if available)
    /// </summary>
    public double? QualityScore { get; set; }
}

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

/// <summary>
/// Result of image validation
/// </summary>
public class ImageValidationResult
{
    /// <summary>
    /// Whether the file is a valid image
    /// </summary>
    public bool IsValidImage { get; set; }

    /// <summary>
    /// Detected image format
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Image dimensions
    /// </summary>
    public ImageDimensions Dimensions { get; set; } = new();

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Color depth (bits per pixel)
    /// </summary>
    public int ColorDepth { get; set; }

    /// <summary>
    /// Whether image has transparency
    /// </summary>
    public bool HasTransparency { get; set; }

    /// <summary>
    /// Image metadata (EXIF, etc.)
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Validation errors if any
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Whether the image is corrupted
    /// </summary>
    public bool IsCorrupted => !IsValidImage || ValidationErrors.Count > 0;
}

/// <summary>
/// Image dimensions
/// </summary>
public class ImageDimensions
{
    public int Width { get; set; }
    public int Height { get; set; }
    public double AspectRatio => Height > 0 ? (double)Width / Height : 0;
}
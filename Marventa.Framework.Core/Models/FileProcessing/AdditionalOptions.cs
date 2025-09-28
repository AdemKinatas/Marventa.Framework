namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Options for image optimization
/// </summary>
public class OptimizationOptions
{
    /// <summary>
    /// Optimization level
    /// </summary>
    public OptimizationLevel Level { get; set; } = OptimizationLevel.Medium;

    /// <summary>
    /// Target quality (1-100)
    /// </summary>
    public int Quality { get; set; } = 85;

    /// <summary>
    /// Whether to preserve metadata
    /// </summary>
    public bool PreserveMetadata { get; set; } = false;

    /// <summary>
    /// Custom optimization parameters
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Whether to enable progressive encoding
    /// </summary>
    public bool EnableProgressive { get; set; } = false;
}

/// <summary>
/// Options for format conversion
/// </summary>
public class ConversionOptions
{
    /// <summary>
    /// Target format
    /// </summary>
    public string TargetFormat { get; set; } = string.Empty;

    /// <summary>
    /// Quality for lossy formats (1-100)
    /// </summary>
    public int Quality { get; set; } = 85;

    /// <summary>
    /// Whether to preserve metadata
    /// </summary>
    public bool PreserveMetadata { get; set; } = false;

    /// <summary>
    /// Custom conversion parameters
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// Options for image validation
/// </summary>
public class ValidationOptions
{
    /// <summary>
    /// Whether to check for corruption
    /// </summary>
    public bool CheckCorruption { get; set; } = true;

    /// <summary>
    /// Whether to validate format
    /// </summary>
    public bool ValidateFormat { get; set; } = true;

    /// <summary>
    /// Whether to check file format
    /// </summary>
    public bool CheckFormat { get; set; } = true;

    /// <summary>
    /// Whether to check dimensions
    /// </summary>
    public bool CheckDimensions { get; set; } = true;

    /// <summary>
    /// Maximum allowed width
    /// </summary>
    public int? MaxWidth { get; set; }

    /// <summary>
    /// Maximum allowed height
    /// </summary>
    public int? MaxHeight { get; set; }

    /// <summary>
    /// Maximum allowed file size in bytes
    /// </summary>
    public long? MaxFileSizeBytes { get; set; }
}

/// <summary>
/// Result of image validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether validation passed
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Image information
    /// </summary>
    public ImageDimensions? Dimensions { get; set; }

    /// <summary>
    /// Detected format
    /// </summary>
    public string? DetectedFormat { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Detected image format
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Color depth in bits per pixel
    /// </summary>
    public int ColorDepth { get; set; }
}

/// <summary>
/// Result of bulk processing operation
/// </summary>
public class BulkProcessingResult
{
    /// <summary>
    /// Total files processed
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Successfully processed files
    /// </summary>
    public int SuccessfullyProcessed { get; set; }

    /// <summary>
    /// Failed processing count
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Processing results by file
    /// </summary>
    public Dictionary<string, ProcessingResult> Results { get; set; } = new();

    /// <summary>
    /// Overall processing time
    /// </summary>
    public TimeSpan TotalProcessingTime { get; set; }

    /// <summary>
    /// Whether bulk operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error details for failed operations
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
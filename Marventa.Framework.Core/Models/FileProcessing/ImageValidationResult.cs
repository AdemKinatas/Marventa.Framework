namespace Marventa.Framework.Core.Models.FileProcessing;

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

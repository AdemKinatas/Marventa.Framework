namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Configuration options for image processing operations
/// </summary>
public class ProcessingOptions
{
    /// <summary>
    /// Target width for resizing (null to maintain aspect ratio)
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Target height for resizing (null to maintain aspect ratio)
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Target width for backward compatibility
    /// </summary>
    public int TargetWidth
    {
        get => Width ?? 0;
        set => Width = value == 0 ? null : value;
    }

    /// <summary>
    /// Target height for backward compatibility
    /// </summary>
    public int TargetHeight
    {
        get => Height ?? 0;
        set => Height = value == 0 ? null : value;
    }

    /// <summary>
    /// Rotation angle in degrees (0, 90, 180, 270)
    /// </summary>
    public int Rotation { get; set; } = 0;

    /// <summary>
    /// Crop rectangle (x, y, width, height)
    /// </summary>
    public CropRectangle? Crop { get; set; }

    /// <summary>
    /// Resize mode (crop, fit, stretch, pad)
    /// </summary>
    public ResizeMode ResizeMode { get; set; } = ResizeMode.Fit;

    /// <summary>
    /// Background color for padding (hex color)
    /// </summary>
    public string BackgroundColor { get; set; } = "#FFFFFF";

    /// <summary>
    /// Quality setting (1-100) for lossy formats
    /// </summary>
    public int Quality { get; set; } = 85;

    /// <summary>
    /// Whether to maintain aspect ratio during resize
    /// </summary>
    public bool MaintainAspectRatio { get; set; } = true;

    /// <summary>
    /// Whether to allow upscaling of images
    /// </summary>
    public bool AllowUpscaling { get; set; } = false;

    /// <summary>
    /// Custom metadata to add to the processed image
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Defines how images should be resized
/// </summary>
public enum ResizeMode
{
    /// <summary>
    /// Resize to fit within bounds, maintaining aspect ratio
    /// </summary>
    Fit,

    /// <summary>
    /// Resize and crop to fill exact dimensions
    /// </summary>
    Crop,

    /// <summary>
    /// Stretch to exact dimensions (may distort)
    /// </summary>
    Stretch,

    /// <summary>
    /// Resize to fit and pad with background color
    /// </summary>
    Pad
}

/// <summary>
/// Defines a rectangular crop area
/// </summary>
public class CropRectangle
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Defines thumbnail size specifications
/// </summary>
public class ThumbnailSize
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public int Quality { get; set; } = 85;
    public string Format { get; set; } = "jpg";
}

/// <summary>
/// Image optimization levels
/// </summary>
public enum OptimizationLevel
{
    /// <summary>
    /// Light optimization, prioritizes quality
    /// </summary>
    Low,

    /// <summary>
    /// Balanced optimization
    /// </summary>
    Medium,

    /// <summary>
    /// Aggressive optimization, smaller file size
    /// </summary>
    High,

    /// <summary>
    /// Maximum compression, may reduce quality significantly
    /// </summary>
    Maximum
}

/// <summary>
/// Watermark positioning and style options
/// </summary>
public class WatermarkOptions
{
    /// <summary>
    /// Watermark image stream or text
    /// </summary>
    public Stream? WatermarkImage { get; set; }

    /// <summary>
    /// Text watermark (if WatermarkImage is null)
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Position on the image
    /// </summary>
    public WatermarkPosition Position { get; set; } = WatermarkPosition.BottomRight;

    /// <summary>
    /// Opacity (0.0 to 1.0)
    /// </summary>
    public float Opacity { get; set; } = 0.7f;

    /// <summary>
    /// Margin from edges in pixels
    /// </summary>
    public int Margin { get; set; } = 10;

    /// <summary>
    /// Scale factor for watermark (0.1 to 1.0)
    /// </summary>
    public float Scale { get; set; } = 0.2f;

    /// <summary>
    /// Font family for text watermarks
    /// </summary>
    public string FontFamily { get; set; } = "Arial";

    /// <summary>
    /// Font size for text watermarks
    /// </summary>
    public int FontSize { get; set; } = 24;

    /// <summary>
    /// Text color (hex format)
    /// </summary>
    public string TextColor { get; set; } = "#FFFFFF";
}

/// <summary>
/// Watermark positioning options
/// </summary>
public enum WatermarkPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}
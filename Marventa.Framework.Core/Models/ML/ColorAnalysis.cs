using Marventa.Framework.Core.Models.FileProcessing;

namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Color analysis result
/// </summary>
public class ColorAnalysisResult
{
    /// <summary>
    /// Dominant colors found in the image
    /// </summary>
    public DominantColor[] DominantColors { get; set; } = Array.Empty<DominantColor>();

    /// <summary>
    /// Color palette as hex strings
    /// </summary>
    public string[] ColorPalette { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Color palette extracted from image
    /// </summary>
    public ColorPalette Palette { get; set; } = new();

    /// <summary>
    /// Color distribution statistics
    /// </summary>
    public ColorStatistics Statistics { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model version used for analysis
    /// </summary>
    public string ModelVersion { get; set; } = string.Empty;

    /// <summary>
    /// Color space used for analysis
    /// </summary>
    public string ColorSpace { get; set; } = string.Empty;

    /// <summary>
    /// Bits per channel in the image
    /// </summary>
    public int BitsPerChannel { get; set; }

    /// <summary>
    /// Whether analysis was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Dominant color with percentage
/// </summary>
public class DominantColor
{
    public string Hex { get; set; } = string.Empty;
    public RGBColor RGB { get; set; } = new();
    public double Percentage { get; set; }
}

/// <summary>
/// Color information with metadata
/// </summary>
public class ColorInfo
{
    public RGBColor RGB { get; set; } = new();
    public HSLColor HSL { get; set; } = new();
    public string HexValue { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public string? ColorName { get; set; }
}

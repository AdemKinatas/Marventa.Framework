namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// URL transformation options
/// </summary>
public class URLTransformations
{
    /// <summary>
    /// Image width for dynamic resizing
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Image height for dynamic resizing
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Image quality (1-100)
    /// </summary>
    public int? Quality { get; set; }

    /// <summary>
    /// Image format (webp, avif, jpg, png)
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Whether to enable smart cropping
    /// </summary>
    public bool? SmartCrop { get; set; }

    /// <summary>
    /// Device pixel ratio for responsive images
    /// </summary>
    public double? DPR { get; set; }

    /// <summary>
    /// Custom transformation parameters
    /// </summary>
    public Dictionary<string, string> CustomParams { get; set; } = new();
}
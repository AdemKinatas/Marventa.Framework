using Marventa.Framework.Core.Models.FileProcessing;

namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Result of tag generation for an image
/// </summary>
public class TagGenerationResult
{
    /// <summary>
    /// Generated tags with confidence scores
    /// </summary>
    public ImageTag[] Tags { get; set; } = Array.Empty<ImageTag>();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model version used for generation
    /// </summary>
    public string ModelVersion { get; set; } = string.Empty;

    /// <summary>
    /// Whether generation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if generation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Image tag with confidence score
/// </summary>
public class ImageTag
{
    public string Tag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Category { get; set; } = string.Empty;
    public BoundingBox? BoundingBox { get; set; }
}

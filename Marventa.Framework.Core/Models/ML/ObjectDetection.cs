using Marventa.Framework.Core.Models.FileProcessing;

namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Object detection result
/// </summary>
public class ObjectDetectionResult
{
    /// <summary>
    /// Detected objects
    /// </summary>
    public DetectedObject[] Objects { get; set; } = Array.Empty<DetectedObject>();

    /// <summary>
    /// Image dimensions
    /// </summary>
    public ImageDimensions ImageDimensions { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model version used for detection
    /// </summary>
    public string ModelVersion { get; set; } = string.Empty;

    /// <summary>
    /// Whether detection was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if detection failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Individual detected object with location
/// </summary>
public class DetectedObject
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public BoundingBox BoundingBox { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalProperties { get; set; } = new();
}

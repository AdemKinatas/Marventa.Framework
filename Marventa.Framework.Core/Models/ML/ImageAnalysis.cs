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

/// <summary>
/// Content moderation analysis result
/// </summary>
public class ModerationResult
{
    /// <summary>
    /// Overall safety score (0.0 = safe, 1.0 = unsafe)
    /// </summary>
    public double OverallScore { get; set; }

    /// <summary>
    /// Overall risk score for the content
    /// </summary>
    public double OverallRiskScore { get; set; }

    /// <summary>
    /// Whether content should be flagged
    /// </summary>
    public bool IsInappropriate { get; set; }

    /// <summary>
    /// Whether content is appropriate for the platform
    /// </summary>
    public bool IsAppropriate { get; set; }

    /// <summary>
    /// Scores by category
    /// </summary>
    public Dictionary<ModerationCategory, double> CategoryScores { get; set; } = new();

    /// <summary>
    /// Detected issues with details
    /// </summary>
    public ModerationIssue[] Issues { get; set; } = Array.Empty<ModerationIssue>();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model version used for moderation
    /// </summary>
    public string ModelVersion { get; set; } = string.Empty;

    /// <summary>
    /// Whether analysis was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Moderation issue details
/// </summary>
public class ModerationIssue
{
    public ModerationCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public ModerationSeverity Severity { get; set; }
    public BoundingBox? Location { get; set; }
}

/// <summary>
/// Quality analysis result for images
/// </summary>
public class QualityAnalysisResult
{
    /// <summary>
    /// Overall quality score (0.0 to 1.0)
    /// </summary>
    public double OverallScore { get; set; }

    /// <summary>
    /// Overall quality score (0.0 to 1.0)
    /// </summary>
    public double OverallQualityScore { get; set; }

    /// <summary>
    /// Individual quality metrics
    /// </summary>
    public QualityMetrics Metrics { get; set; } = new();

    /// <summary>
    /// Blur detection score (0.0 = sharp, 1.0 = very blurry)
    /// </summary>
    public double BlurScore { get; set; }

    /// <summary>
    /// Noise level score (0.0 = clean, 1.0 = very noisy)
    /// </summary>
    public double NoiseScore { get; set; }

    /// <summary>
    /// Exposure quality (0.0 = poor, 1.0 = excellent)
    /// </summary>
    public double ExposureScore { get; set; }

    /// <summary>
    /// Contrast level (0.0 = poor, 1.0 = excellent)
    /// </summary>
    public double ContrastScore { get; set; }

    /// <summary>
    /// Sharpness score
    /// </summary>
    public double SharpnessScore { get; set; }

    /// <summary>
    /// Color balance score
    /// </summary>
    public double ColorBalance { get; set; }

    /// <summary>
    /// Resolution adequacy score
    /// </summary>
    public double ResolutionScore { get; set; }

    /// <summary>
    /// Image composition score
    /// </summary>
    public double CompositionScore { get; set; }

    /// <summary>
    /// Quality issues detected
    /// </summary>
    public QualityIssue[] Issues { get; set; } = Array.Empty<QualityIssue>();

    /// <summary>
    /// Suggestions for improvement
    /// </summary>
    public string[] Suggestions { get; set; } = Array.Empty<string>();

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
    /// Recommendations for quality improvement
    /// </summary>
    public string[] Recommendations { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether analysis was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Detailed quality metrics
/// </summary>
public class QualityMetrics
{
    /// <summary>
    /// Blur detection score (0.0 = sharp, 1.0 = very blurry)
    /// </summary>
    public double BlurScore { get; set; }

    /// <summary>
    /// Noise level score (0.0 = clean, 1.0 = very noisy)
    /// </summary>
    public double NoiseScore { get; set; }

    /// <summary>
    /// Exposure quality (0.0 = poor, 1.0 = excellent)
    /// </summary>
    public double ExposureScore { get; set; }

    /// <summary>
    /// Contrast level (0.0 = poor, 1.0 = excellent)
    /// </summary>
    public double ContrastScore { get; set; }

    /// <summary>
    /// Color saturation quality
    /// </summary>
    public double SaturationScore { get; set; }

    /// <summary>
    /// Image composition score
    /// </summary>
    public double CompositionScore { get; set; }

    /// <summary>
    /// Resolution adequacy score
    /// </summary>
    public double ResolutionScore { get; set; }
}

/// <summary>
/// Quality issue description
/// </summary>
public class QualityIssue
{
    public QualityIssueType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Severity { get; set; }
    public string Suggestion { get; set; } = string.Empty;
}

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
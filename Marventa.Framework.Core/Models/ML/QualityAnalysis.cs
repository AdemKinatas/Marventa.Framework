namespace Marventa.Framework.Core.Models.ML;

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

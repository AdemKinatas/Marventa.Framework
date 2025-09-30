using Marventa.Framework.Core.Models.FileProcessing;

namespace Marventa.Framework.Core.Models.ML;

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

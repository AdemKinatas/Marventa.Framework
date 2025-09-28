namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Alt text generation result
/// </summary>
public class AltTextGenerationResult
{
    public string AltText { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string[] Keywords { get; set; } = Array.Empty<string>();
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
    /// Language of the generated text
    /// </summary>
    public string Language { get; set; } = string.Empty;

    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Content optimization suggestions
/// </summary>
public class ContentOptimizationResult
{
    public OptimizationSuggestion[] Suggestions { get; set; } = Array.Empty<OptimizationSuggestion>();
    public double OverallScore { get; set; }
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Target audience for optimization
    /// </summary>
    public string TargetAudience { get; set; } = string.Empty;

    public bool Success { get; set; }
}

/// <summary>
/// Individual optimization suggestion
/// </summary>
public class OptimizationSuggestion
{
    public string Category { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
    public double Impact { get; set; }
    public string Priority { get; set; } = string.Empty;
}

/// <summary>
/// Quality analysis options
/// </summary>
public class QualityAnalysisOptions
{
    public bool AnalyzeBlur { get; set; } = true;
    public bool AnalyzeNoise { get; set; } = true;
    public bool AnalyzeExposure { get; set; } = true;
    public bool AnalyzeContrast { get; set; } = true;
    public bool AnalyzeComposition { get; set; } = false;
    public double MinimumResolution { get; set; } = 0.5;
}

/// <summary>
/// Batch processing result
/// </summary>
public class BatchProcessingResult
{
    public Dictionary<string, Dictionary<MLOperation, object>> Results { get; set; } = new();
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public long TotalProcessingTimeMs { get; set; }
    public bool OverallSuccess { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    // Additional properties for compatibility
    public int TotalProcessed { get; set; }
    public int TotalImages { get; set; }
    public int SuccessfulProcessing { get; set; }
    public int FailedProcessing { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public bool Success { get; set; }
}
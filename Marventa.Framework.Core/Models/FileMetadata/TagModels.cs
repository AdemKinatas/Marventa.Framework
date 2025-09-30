using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Models.FileMetadata;

/// <summary>
/// Result of a tag operation (add/remove)
/// </summary>
public class TagOperationResult
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that was operated on
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Tags that were successfully processed
    /// </summary>
    public FileTag[] ProcessedTags { get; set; } = Array.Empty<FileTag>();

    /// <summary>
    /// Tags that failed to process
    /// </summary>
    public List<string> FailedTags { get; set; } = new();

    /// <summary>
    /// Current tags on the file after operation
    /// </summary>
    public List<FileTag> CurrentTags { get; set; } = new();

    /// <summary>
    /// Total count of tags after operation
    /// </summary>
    public int TotalTagCount { get; set; }

    /// <summary>
    /// When the operation was completed
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Time taken for the operation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Filters for tag analysis
/// </summary>
public class TagFilters
{
    /// <summary>
    /// Filter by tag source
    /// </summary>
    public List<TagSource> Sources { get; set; } = new();

    /// <summary>
    /// Filter by content types
    /// </summary>
    public List<string> ContentTypes { get; set; } = new();

    /// <summary>
    /// Filter by user who created the tag
    /// </summary>
    public List<string> CreatedBy { get; set; } = new();

    /// <summary>
    /// Minimum confidence score for AI-generated tags
    /// </summary>
    public double? MinConfidence { get; set; }

    /// <summary>
    /// Date range for when tags were created
    /// </summary>
    public TimeRange? CreatedRange { get; set; }

    /// <summary>
    /// Exclude these specific tags
    /// </summary>
    public List<string> ExcludeTags { get; set; } = new();
}

/// <summary>
/// Result of tag popularity analysis
/// </summary>
public class TagPopularityResult
{
    /// <summary>
    /// Whether the analysis was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the analysis failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Popular tags with usage statistics
    /// </summary>
    public List<TagStatistics> PopularTags { get; set; } = new();

    /// <summary>
    /// Time range that was analyzed
    /// </summary>
    public TimeRange? TimeRange { get; set; }

    /// <summary>
    /// Filters that were applied
    /// </summary>
    public TagFilters? Filters { get; set; }

    /// <summary>
    /// Total number of unique tags analyzed
    /// </summary>
    public int TotalUniqueTagsAnalyzed { get; set; }

    /// <summary>
    /// Total unique tag count across all analysis
    /// </summary>
    public int TotalUniqueTagCount { get; set; }

    /// <summary>
    /// Time taken for the analysis
    /// </summary>
    public TimeSpan AnalysisTime { get; set; }
}

/// <summary>
/// Tag usage statistics
/// </summary>
public class TagStatistics
{
    /// <summary>
    /// The tag name
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Number of files using this tag
    /// </summary>
    public long UsageCount { get; set; }

    /// <summary>
    /// Percentage of total files using this tag
    /// </summary>
    public double UsagePercentage { get; set; }

    /// <summary>
    /// Trend for this tag
    /// </summary>
    public TagTrend Trend { get; set; }

    /// <summary>
    /// Sources of this tag
    /// </summary>
    public Dictionary<TagSource, long> SourceBreakdown { get; set; } = new();

    /// <summary>
    /// Recent usage pattern
    /// </summary>
    public Dictionary<DateTime, long> RecentUsage { get; set; } = new();

    /// <summary>
    /// Average confidence for AI-generated instances of this tag
    /// </summary>
    public double? AverageConfidence { get; set; }
}

/// <summary>
/// Result of tag suggestion operation
/// </summary>
public class TagSuggestionResult
{
    /// <summary>
    /// Whether the suggestion generation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the suggestion failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that suggestions were generated for
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Suggested tags with confidence scores
    /// </summary>
    public List<TagSuggestion> Suggestions { get; set; } = new();

    /// <summary>
    /// Minimum confidence that was required
    /// </summary>
    public double MinConfidence { get; set; }

    /// <summary>
    /// Maximum number of suggestions that were requested
    /// </summary>
    public int MaxSuggestions { get; set; }

    /// <summary>
    /// Time taken for suggestion generation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Sources used for generating suggestions
    /// </summary>
    public List<TagSuggestionSource> SourcesUsed { get; set; } = new();
}

/// <summary>
/// Tag suggestion with confidence
/// </summary>
public class TagSuggestion
{
    /// <summary>
    /// Suggested tag
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Name of the suggested tag
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score (0.0 to 1.0)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Source of the suggestion
    /// </summary>
    public TagSuggestionSource Source { get; set; }

    /// <summary>
    /// Reason for the suggestion
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Additional context for the suggestion
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();
}

using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Models.FileMetadata;

/// <summary>
/// Result of a metadata storage operation
/// </summary>
public class MetadataStorageResult
{
    /// <summary>
    /// Whether the storage operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that was stored
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Metadata ID generated for this storage operation
    /// </summary>
    public string MetadataId { get; set; } = string.Empty;

    /// <summary>
    /// When the metadata was stored
    /// </summary>
    public DateTime StoredAt { get; set; }

    /// <summary>
    /// Number of metadata properties stored
    /// </summary>
    public int PropertiesStored { get; set; }

    /// <summary>
    /// Number of tags stored
    /// </summary>
    public int TagsStored { get; set; }

    /// <summary>
    /// Time taken for the operation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Additional operation details
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// Result of a metadata update operation
/// </summary>
public class MetadataUpdateResult
{
    /// <summary>
    /// Whether the update operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that was updated
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Number of properties that were modified
    /// </summary>
    public int PropertiesModified { get; set; }

    /// <summary>
    /// Number of tags that were modified
    /// </summary>
    public int TagsModified { get; set; }

    /// <summary>
    /// Previous version of the metadata
    /// </summary>
    public FileMetadata? PreviousMetadata { get; set; }

    /// <summary>
    /// Updated metadata
    /// </summary>
    public FileMetadata? UpdatedMetadata { get; set; }

    /// <summary>
    /// Changes that were applied
    /// </summary>
    public List<MetadataChange> Changes { get; set; } = new();

    /// <summary>
    /// When the metadata was updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Previous version identifier
    /// </summary>
    public string PreviousVersion { get; set; } = string.Empty;

    /// <summary>
    /// New version identifier
    /// </summary>
    public string NewVersion { get; set; } = string.Empty;

    /// <summary>
    /// Time taken for the operation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Result of a metadata deletion operation
/// </summary>
public class MetadataDeletionResult
{
    /// <summary>
    /// Whether the deletion operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that was deleted
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Whether the file was found before deletion
    /// </summary>
    public bool WasFound { get; set; }

    /// <summary>
    /// Backup of the deleted metadata
    /// </summary>
    public FileMetadata? DeletedMetadata { get; set; }

    /// <summary>
    /// When the metadata was deleted
    /// </summary>
    public DateTime DeletedAt { get; set; }

    /// <summary>
    /// Time taken for the operation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Item in metadata search results
/// </summary>
public class MetadataSearchResultItem
{
    /// <summary>
    /// File ID
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// File title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// File description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// File tags
    /// </summary>
    public FileTag[] Tags { get; set; } = Array.Empty<FileTag>();

    /// <summary>
    /// Relevance score
    /// </summary>
    public double Relevance { get; set; }

    /// <summary>
    /// When the file was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Item in similarity search results
/// </summary>
public class SimilarityResultItem
{
    /// <summary>
    /// File ID
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// File title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Similarity score
    /// </summary>
    public double SimilarityScore { get; set; }

    /// <summary>
    /// File tags
    /// </summary>
    public FileTag[] Tags { get; set; } = Array.Empty<FileTag>();

    /// <summary>
    /// When the file was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Search criteria for metadata searches
/// </summary>
public class MetadataSearchCriteria
{
    /// <summary>
    /// Tags to search for
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Whether all tags must match (AND) or any tag (OR)
    /// </summary>
    public bool RequireAllTags { get; set; } = false;

    /// <summary>
    /// Content type filters
    /// </summary>
    public List<string> ContentTypes { get; set; } = new();

    /// <summary>
    /// File size range
    /// </summary>
    public FileSizeRange? SizeRange { get; set; }

    /// <summary>
    /// Creation date range
    /// </summary>
    public TimeRange? CreatedRange { get; set; }

    /// <summary>
    /// Modified date range
    /// </summary>
    public TimeRange? ModifiedRange { get; set; }

    /// <summary>
    /// User who uploaded the file
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Custom property filters
    /// </summary>
    public Dictionary<string, object> PropertyFilters { get; set; } = new();

    /// <summary>
    /// Sort order for results
    /// </summary>
    public SearchSortOrder SortOrder { get; set; } = SearchSortOrder.Relevance;

    /// <summary>
    /// Maximum number of results to return
    /// </summary>
    public int MaxResults { get; set; } = 100;

    /// <summary>
    /// Skip this many results (for pagination)
    /// </summary>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Include file analytics in results
    /// </summary>
    public bool IncludeAnalytics { get; set; } = false;
}

/// <summary>
/// Result of a metadata search operation
/// </summary>
public class MetadataSearchResult
{
    /// <summary>
    /// Whether the search was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the search failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Search result items
    /// </summary>
    public MetadataSearchResultItem[] Items { get; set; } = Array.Empty<MetadataSearchResultItem>();

    /// <summary>
    /// Total number of results
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Files that matched the search criteria
    /// </summary>
    public List<FileMetadata> MatchingFiles { get; set; } = new();

    /// <summary>
    /// Total number of files that matched (before pagination)
    /// </summary>
    public int TotalMatches { get; set; }

    /// <summary>
    /// Number of files returned in this result
    /// </summary>
    public int ReturnedCount { get; set; }

    /// <summary>
    /// Whether there are more results available
    /// </summary>
    public bool HasMoreResults { get; set; }

    /// <summary>
    /// Search criteria that was used
    /// </summary>
    public MetadataSearchCriteria SearchCriteria { get; set; } = new();

    /// <summary>
    /// Time taken for the search
    /// </summary>
    public TimeSpan SearchTime { get; set; }

    /// <summary>
    /// Search facets and aggregations
    /// </summary>
    public Dictionary<string, object> Facets { get; set; } = new();
}

/// <summary>
/// Result of a similarity search operation
/// </summary>
public class SimilaritySearchResult
{
    /// <summary>
    /// Whether the search was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the search failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Reference file ID used for comparison
    /// </summary>
    public string ReferenceFileId { get; set; } = string.Empty;

    /// <summary>
    /// Similar files with similarity scores
    /// </summary>
    public List<SimilarityMatch> SimilarFiles { get; set; } = new();

    /// <summary>
    /// Similarity threshold that was used
    /// </summary>
    public double SimilarityThreshold { get; set; }

    /// <summary>
    /// Types of similarity that were analyzed
    /// </summary>
    public List<SimilarityType> SimilarityTypes { get; set; } = new();

    /// <summary>
    /// Time taken for the search
    /// </summary>
    public TimeSpan SearchTime { get; set; }
}

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
/// Result of file analytics analysis
/// </summary>
public class FileAnalyticsResult
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
    /// File ID that was analyzed (null for global analytics)
    /// </summary>
    public string? FileId { get; set; }

    /// <summary>
    /// Time range that was analyzed
    /// </summary>
    public TimeRange? TimeRange { get; set; }

    /// <summary>
    /// Total number of accesses
    /// </summary>
    public long TotalAccesses { get; set; }

    /// <summary>
    /// Unique users who accessed the file(s)
    /// </summary>
    public int UniqueUsers { get; set; }

    /// <summary>
    /// Access patterns by type
    /// </summary>
    public Dictionary<FileAccessType, long> AccessByType { get; set; } = new();

    /// <summary>
    /// Access patterns over time
    /// </summary>
    public Dictionary<DateTime, long> AccessOverTime { get; set; } = new();

    /// <summary>
    /// Peak access time
    /// </summary>
    public DateTime? PeakAccessTime { get; set; }

    /// <summary>
    /// Most active users
    /// </summary>
    public List<UserAccessStatistics> MostActiveUsers { get; set; } = new();

    /// <summary>
    /// Access patterns by geographic location (if available)
    /// </summary>
    public Dictionary<string, long> AccessByLocation { get; set; } = new();

    /// <summary>
    /// Average time between accesses
    /// </summary>
    public TimeSpan? AverageTimeBetweenAccesses { get; set; }

    /// <summary>
    /// Total number of files in analytics
    /// </summary>
    public long TotalFiles { get; set; }

    /// <summary>
    /// Total number of views across all files
    /// </summary>
    public long TotalViews { get; set; }

    /// <summary>
    /// Total number of downloads across all files
    /// </summary>
    public long TotalDownloads { get; set; }

    /// <summary>
    /// Average rating across all files
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Most frequently used tags
    /// </summary>
    public List<string> TopTags { get; set; } = new();

    /// <summary>
    /// Distribution of files by type
    /// </summary>
    public Dictionary<string, long> FileTypeDistribution { get; set; } = new();

    /// <summary>
    /// Time taken for the analysis
    /// </summary>
    public TimeSpan AnalysisTime { get; set; }
}

/// <summary>
/// Result of access recording operation
/// </summary>
public class AccessRecordingResult
{
    /// <summary>
    /// Whether the recording was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the recording failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that access was recorded for
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for this access record
    /// </summary>
    public string AccessId { get; set; } = string.Empty;

    /// <summary>
    /// Type of access that was recorded
    /// </summary>
    public FileAccessType AccessType { get; set; }

    /// <summary>
    /// User who accessed the file
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Timestamp when access was recorded
    /// </summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>
    /// Total access count for this file after recording
    /// </summary>
    public long TotalAccessCount { get; set; }

    /// <summary>
    /// Time taken for the recording operation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Options for bulk import operations
/// </summary>
public class BulkImportOptions
{
    /// <summary>
    /// How to handle conflicts with existing metadata
    /// </summary>
    public MetadataMergeMode ConflictResolution { get; set; } = MetadataMergeMode.Merge;

    /// <summary>
    /// Whether to validate metadata before importing
    /// </summary>
    public bool ValidateBeforeImport { get; set; } = true;

    /// <summary>
    /// Maximum number of concurrent import operations
    /// </summary>
    public int MaxConcurrency { get; set; } = 10;

    /// <summary>
    /// Whether to continue import if some items fail
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Batch size for processing imports
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Whether to create backup before import
    /// </summary>
    public bool CreateBackup { get; set; } = false;

    /// <summary>
    /// Additional import settings
    /// </summary>
    public Dictionary<string, object> AdditionalSettings { get; set; } = new();
}

/// <summary>
/// Result of bulk import operation
/// </summary>
public class BulkImportResult
{
    /// <summary>
    /// Whether the overall import was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the import failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Total number of items attempted
    /// </summary>
    public int TotalAttempted { get; set; }

    /// <summary>
    /// Total number of entries in the import operation
    /// </summary>
    public int TotalEntries { get; set; }

    /// <summary>
    /// Number of items successfully imported
    /// </summary>
    public int SuccessfulImports { get; set; }

    /// <summary>
    /// Number of items that failed to import
    /// </summary>
    public int FailedImports { get; set; }

    /// <summary>
    /// Number of items that were skipped
    /// </summary>
    public int SkippedImports { get; set; }

    /// <summary>
    /// Details of failed imports
    /// </summary>
    public List<ImportFailure> Failures { get; set; } = new();

    /// <summary>
    /// Individual import results for each item
    /// </summary>
    public List<MetadataStorageResult> Results { get; set; } = new();

    /// <summary>
    /// Import options that were used
    /// </summary>
    public BulkImportOptions ImportOptions { get; set; } = new();

    /// <summary>
    /// Time taken for the entire import
    /// </summary>
    public TimeSpan TotalProcessingTime { get; set; }

    /// <summary>
    /// Import time for tracking when the operation completed
    /// </summary>
    public DateTime ImportTime { get; set; }

    /// <summary>
    /// Statistics about the import
    /// </summary>
    public Dictionary<string, object> Statistics { get; set; } = new();
}

/// <summary>
/// Criteria for metadata export
/// </summary>
public class MetadataExportCriteria
{
    /// <summary>
    /// Specific file IDs to export (if null, export all based on other criteria)
    /// </summary>
    public List<string>? FileIds { get; set; }

    /// <summary>
    /// Tags to filter by
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Content types to include
    /// </summary>
    public List<string> ContentTypes { get; set; } = new();

    /// <summary>
    /// Date range for file creation
    /// </summary>
    public TimeRange? CreatedRange { get; set; }

    /// <summary>
    /// Date range for file modification
    /// </summary>
    public TimeRange? ModifiedRange { get; set; }

    /// <summary>
    /// File size range
    /// </summary>
    public FileSizeRange? SizeRange { get; set; }

    /// <summary>
    /// User who uploaded files
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Whether to include file analytics data
    /// </summary>
    public bool IncludeAnalytics { get; set; } = false;

    /// <summary>
    /// Whether to include custom properties
    /// </summary>
    public bool IncludeCustomProperties { get; set; } = true;

    /// <summary>
    /// Whether to include tags
    /// </summary>
    public bool IncludeTags { get; set; } = true;

    /// <summary>
    /// Maximum number of files to export
    /// </summary>
    public int? MaxFiles { get; set; }
}

/// <summary>
/// Result of metadata export operation
/// </summary>
public class MetadataExportResult
{
    /// <summary>
    /// Whether the export was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the export failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Export format that was used
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Stream containing the exported data
    /// </summary>
    public Stream? DataStream { get; set; }

    /// <summary>
    /// Size of exported data in bytes
    /// </summary>
    public long DataSizeBytes { get; set; }

    /// <summary>
    /// Number of files that were exported
    /// </summary>
    public int ExportedFileCount { get; set; }

    /// <summary>
    /// Total number of metadata records exported
    /// </summary>
    public int TotalExported { get; set; }

    /// <summary>
    /// Export criteria that was used
    /// </summary>
    public MetadataExportCriteria ExportCriteria { get; set; } = new();

    /// <summary>
    /// Time taken for the export
    /// </summary>
    public TimeSpan ExportTime { get; set; }

    /// <summary>
    /// Additional export metadata
    /// </summary>
    public Dictionary<string, object> ExportMetadata { get; set; } = new();
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

// Supporting classes

/// <summary>
/// Represents a change in metadata
/// </summary>
public class MetadataChange
{
    /// <summary>
    /// Type of change
    /// </summary>
    public MetadataChangeType ChangeType { get; set; }

    /// <summary>
    /// Property that was changed
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Previous value
    /// </summary>
    public object? OldValue { get; set; }

    /// <summary>
    /// New value
    /// </summary>
    public object? NewValue { get; set; }

    /// <summary>
    /// When the change was made
    /// </summary>
    public DateTime ChangedAt { get; set; }
}

/// <summary>
/// File size range for filtering
/// </summary>
public class FileSizeRange
{
    /// <summary>
    /// Minimum file size in bytes (inclusive)
    /// </summary>
    public long? MinBytes { get; set; }

    /// <summary>
    /// Maximum file size in bytes (inclusive)
    /// </summary>
    public long? MaxBytes { get; set; }
}

/// <summary>
/// Similarity match result
/// </summary>
public class SimilarityMatch
{
    /// <summary>
    /// File metadata for the similar file
    /// </summary>
    public FileMetadata FileMetadata { get; set; } = new();

    /// <summary>
    /// Overall similarity score (0.0 to 1.0)
    /// </summary>
    public double SimilarityScore { get; set; }

    /// <summary>
    /// Breakdown of similarity by type
    /// </summary>
    public Dictionary<SimilarityType, double> SimilarityBreakdown { get; set; } = new();

    /// <summary>
    /// Reasons why this file is considered similar
    /// </summary>
    public List<string> SimilarityReasons { get; set; } = new();
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
/// User access statistics
/// </summary>
public class UserAccessStatistics
{
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Total number of accesses by this user
    /// </summary>
    public long AccessCount { get; set; }

    /// <summary>
    /// Last access time
    /// </summary>
    public DateTime LastAccess { get; set; }

    /// <summary>
    /// Access breakdown by type
    /// </summary>
    public Dictionary<FileAccessType, long> AccessByType { get; set; } = new();

    /// <summary>
    /// Most accessed files by this user
    /// </summary>
    public List<string> TopAccessedFiles { get; set; } = new();
}

/// <summary>
/// Import failure details
/// </summary>
public class ImportFailure
{
    /// <summary>
    /// File ID that failed to import
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Exception details if available
    /// </summary>
    public string? ExceptionDetails { get; set; }

    /// <summary>
    /// Metadata that failed to import
    /// </summary>
    public FileMetadata? FailedMetadata { get; set; }
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

/// <summary>
/// Types of metadata changes
/// </summary>
public enum MetadataChangeType
{
    Added,
    Modified,
    Removed,
    Renamed
}
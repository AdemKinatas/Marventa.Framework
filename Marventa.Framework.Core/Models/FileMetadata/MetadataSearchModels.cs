using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Models.FileMetadata;

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

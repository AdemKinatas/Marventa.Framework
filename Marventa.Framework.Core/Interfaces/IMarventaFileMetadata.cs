using Marventa.Framework.Core.Models.CDN;
using Marventa.Framework.Core.Models.FileMetadata;

namespace Marventa.Framework.Core.Interfaces;

/// <summary>
/// Provides advanced file metadata management with search, tagging, and analytics capabilities
/// </summary>
public interface IMarventaFileMetadata
{
    /// <summary>
    /// Stores comprehensive metadata for a file
    /// </summary>
    /// <param name="fileId">Unique file identifier</param>
    /// <param name="metadata">File metadata to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage result with success status</returns>
    Task<MetadataStorageResult> StoreMetadataAsync(string fileId, FileMetadata metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves metadata for a specific file
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="includeAnalytics">Whether to include access analytics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File metadata with optional analytics</returns>
    Task<FileMetadata?> GetMetadataAsync(string fileId, bool includeAnalytics = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates existing metadata for a file
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="metadata">Updated metadata</param>
    /// <param name="mergeMode">How to merge with existing metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update result with success status</returns>
    Task<MetadataUpdateResult> UpdateMetadataAsync(string fileId, FileMetadata metadata, MetadataMergeMode mergeMode = MetadataMergeMode.Merge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes metadata for a file
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion result</returns>
    Task<MetadataDeletionResult> DeleteMetadataAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches files by tags with advanced filtering
    /// </summary>
    /// <param name="searchCriteria">Search criteria including tags, filters, and sorting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results with matching files</returns>
    Task<MetadataSearchResult> SearchByTagsAsync(MetadataSearchCriteria searchCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches files by content similarity (semantic search)
    /// </summary>
    /// <param name="referenceFileId">Reference file for similarity comparison</param>
    /// <param name="similarityThreshold">Minimum similarity score (0.0 to 1.0)</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Similar files with similarity scores</returns>
    Task<SimilaritySearchResult> SearchBySimilarityAsync(string referenceFileId, double similarityThreshold = 0.7, int maxResults = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds tags to a file's metadata
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="tags">Tags to add</param>
    /// <param name="source">Source of the tags (user, ai, system)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tag addition result</returns>
    Task<TagOperationResult> AddTagsAsync(string fileId, string[] tags, TagSource source = TagSource.User, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes tags from a file's metadata
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="tags">Tags to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tag removal result</returns>
    Task<TagOperationResult> RemoveTagsAsync(string fileId, string[] tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most popular tags across all files
    /// </summary>
    /// <param name="limit">Maximum number of tags to return</param>
    /// <param name="timeRange">Time range for tag popularity</param>
    /// <param name="filters">Optional filters for tag analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Popular tags with usage statistics</returns>
    Task<TagPopularityResult> GetPopularTagsAsync(int limit = 50, TimeRange? timeRange = null, TagFilters? filters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes file access patterns and usage statistics
    /// </summary>
    /// <param name="fileId">File identifier (null for all files)</param>
    /// <param name="timeRange">Time range for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Usage analytics and patterns</returns>
    Task<FileAnalyticsResult> GetFileAnalyticsAsync(string? fileId = null, TimeRange? timeRange = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records file access for analytics tracking
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="accessType">Type of access (view, download, edit, etc.)</param>
    /// <param name="userId">User who accessed the file</param>
    /// <param name="metadata">Additional access metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recording result</returns>
    Task<AccessRecordingResult> RecordAccessAsync(string fileId, FileAccessType accessType, string? userId = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk imports metadata for multiple files
    /// </summary>
    /// <param name="metadataEntries">Dictionary of file IDs to metadata</param>
    /// <param name="importOptions">Import configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk import result with success/failure details</returns>
    Task<BulkImportResult> BulkImportMetadataAsync(Dictionary<string, FileMetadata> metadataEntries, BulkImportOptions? importOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports metadata for backup or migration
    /// </summary>
    /// <param name="exportCriteria">Criteria for selecting files to export</param>
    /// <param name="format">Export format (json, csv, xml)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with data stream</returns>
    Task<MetadataExportResult> ExportMetadataAsync(MetadataExportCriteria exportCriteria, string format = "json", CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggests tags for a file based on content analysis and similar files
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="maxSuggestions">Maximum number of tag suggestions</param>
    /// <param name="confidence">Minimum confidence for suggestions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tag suggestions with confidence scores</returns>
    Task<TagSuggestionResult> SuggestTagsAsync(string fileId, int maxSuggestions = 10, double confidence = 0.5, CancellationToken cancellationToken = default);
}
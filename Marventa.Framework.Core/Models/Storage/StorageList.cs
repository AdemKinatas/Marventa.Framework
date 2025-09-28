namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Options for listing files
/// </summary>
public class StorageListOptions
{
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 100;

    /// <summary>
    /// Continuation token for pagination
    /// </summary>
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// File name prefix filter
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// File extension filter
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// Date range filter
    /// </summary>
    public DateRange? DateRange { get; set; }

    /// <summary>
    /// Sort criteria
    /// </summary>
    public StorageSortBy SortBy { get; set; } = StorageSortBy.Name;

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Whether to include subdirectories
    /// </summary>
    public bool Recursive { get; set; } = false;

    /// <summary>
    /// Whether to include file metadata
    /// </summary>
    public bool IncludeMetadata { get; set; } = false;
}

/// <summary>
/// Result of file listing operation
/// </summary>
public class StorageListResult
{
    /// <summary>
    /// List of files
    /// </summary>
    public StorageFileInfo[] Files { get; set; } = Array.Empty<StorageFileInfo>();

    /// <summary>
    /// Total count of files (may be more than returned due to pagination)
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Continuation token for next page
    /// </summary>
    public string? NextContinuationToken { get; set; }

    /// <summary>
    /// Whether there are more results
    /// </summary>
    public bool HasMoreResults { get; set; }

    /// <summary>
    /// Directory path that was listed
    /// </summary>
    public string DirectoryPath { get; set; } = string.Empty;
}

/// <summary>
/// Directory operation result
/// </summary>
public class StorageDirectoryResult
{
    /// <summary>
    /// Directory path
    /// </summary>
    public string DirectoryPath { get; set; } = string.Empty;

    /// <summary>
    /// Whether operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Date range for filtering
/// </summary>
public class DateRange
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
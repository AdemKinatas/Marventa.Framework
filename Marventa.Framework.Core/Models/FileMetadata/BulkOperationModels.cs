using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Models.FileMetadata;

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

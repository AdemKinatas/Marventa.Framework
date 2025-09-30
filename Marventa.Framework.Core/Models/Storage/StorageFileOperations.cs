namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Options for storage deletion operations
/// </summary>
public class StorageDeletionOptions
{
    /// <summary>
    /// Specific version to delete (if versioning enabled)
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Whether to bypass recycle bin (permanent delete)
    /// </summary>
    public bool PermanentDelete { get; set; } = false;

    /// <summary>
    /// Whether to delete all versions (if versioning enabled)
    /// </summary>
    public bool DeleteAllVersions { get; set; } = false;
}

/// <summary>
/// Result of storage deletion operation
/// </summary>
public class StorageDeletionResult
{
    public string FileId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime DeletedAt { get; set; }

    /// <summary>
    /// Whether file was moved to recycle bin or permanently deleted
    /// </summary>
    public bool PermanentlyDeleted { get; set; }

    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Options for copy operations
/// </summary>
public class StorageCopyOptions
{
    public bool OverwriteIfExists { get; set; } = false;
    public bool PreserveMetadata { get; set; } = true;

    /// <summary>
    /// New metadata to apply to the copy
    /// </summary>
    public Dictionary<string, string>? NewMetadata { get; set; }

    public StorageClass? StorageClass { get; set; }
}

/// <summary>
/// Result of copy operation
/// </summary>
public class StorageCopyResult
{
    public string NewFileId { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public bool Success { get; set; }

    /// <summary>
    /// Copy timestamp
    /// </summary>
    public DateTime CopiedAt { get; set; }

    /// <summary>
    /// Error message if copy failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Options for move operations
/// </summary>
public class StorageMoveOptions
{
    /// <summary>
    /// Whether to overwrite destination if it exists
    /// </summary>
    public bool OverwriteIfExists { get; set; } = false;

    /// <summary>
    /// Whether to preserve metadata
    /// </summary>
    public bool PreserveMetadata { get; set; } = true;
}

/// <summary>
/// Result of move operation
/// </summary>
public class StorageMoveResult
{
    /// <summary>
    /// File identifier (unchanged)
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// New file path
    /// </summary>
    public string NewPath { get; set; } = string.Empty;

    /// <summary>
    /// Whether move was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Move timestamp
    /// </summary>
    public DateTime MovedAt { get; set; }

    /// <summary>
    /// Error message if move failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
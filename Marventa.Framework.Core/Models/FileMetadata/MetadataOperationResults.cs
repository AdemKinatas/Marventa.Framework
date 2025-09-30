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

namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// File information in storage
/// </summary>
public class StorageFileInfo
{
    /// <summary>
    /// Unique file identifier
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Full file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Content type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// ETag for caching
    /// </summary>
    public string? ETag { get; set; }

    /// <summary>
    /// MD5 hash
    /// </summary>
    public string? MD5Hash { get; set; }

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Access control information
    /// </summary>
    public StorageAccessControl? AccessControl { get; set; }

    /// <summary>
    /// Storage class/tier
    /// </summary>
    public StorageClass StorageClass { get; set; }

    /// <summary>
    /// Version information (if versioning enabled)
    /// </summary>
    public string? VersionId { get; set; }
}
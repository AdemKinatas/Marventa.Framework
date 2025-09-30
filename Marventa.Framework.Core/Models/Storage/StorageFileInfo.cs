namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// File information in storage
/// </summary>
public class StorageFileInfo
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }

    /// <summary>
    /// ETag for caching
    /// </summary>
    public string? ETag { get; set; }

    public string? MD5Hash { get; set; }

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Access control information
    /// </summary>
    public StorageAccessControl? AccessControl { get; set; }

    public StorageClass StorageClass { get; set; }

    /// <summary>
    /// Version information (if versioning enabled)
    /// </summary>
    public string? VersionId { get; set; }
}
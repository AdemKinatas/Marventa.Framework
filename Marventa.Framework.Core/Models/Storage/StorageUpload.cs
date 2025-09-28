namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Options for storage upload operations
/// </summary>
public class StorageUploadOptions
{
    /// <summary>
    /// Directory path where file should be stored
    /// </summary>
    public string? DirectoryPath { get; set; }

    /// <summary>
    /// Custom metadata to associate with the file
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Access control settings
    /// </summary>
    public StorageAccessControl? AccessControl { get; set; }

    /// <summary>
    /// Whether to overwrite if file exists
    /// </summary>
    public bool OverwriteIfExists { get; set; } = false;

    /// <summary>
    /// Whether to generate thumbnails automatically
    /// </summary>
    public bool GenerateThumbnails { get; set; } = false;

    /// <summary>
    /// Whether to enable versioning
    /// </summary>
    public bool EnableVersioning { get; set; } = false;

    /// <summary>
    /// Server-side encryption settings
    /// </summary>
    public EncryptionOptions? Encryption { get; set; }

    /// <summary>
    /// Storage class/tier for the file
    /// </summary>
    public StorageClass StorageClass { get; set; } = StorageClass.Standard;
}

/// <summary>
/// Result of storage upload operation
/// </summary>
public class StorageUploadResult
{
    /// <summary>
    /// Unique file identifier
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Full path to the uploaded file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Public URL for accessing the file (if applicable)
    /// </summary>
    public string? PublicUrl { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// MD5 hash of the file content
    /// </summary>
    public string? MD5Hash { get; set; }

    /// <summary>
    /// ETag for caching
    /// </summary>
    public string? ETag { get; set; }

    /// <summary>
    /// Upload timestamp
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Version identifier (if versioning enabled)
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Whether upload was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if upload failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
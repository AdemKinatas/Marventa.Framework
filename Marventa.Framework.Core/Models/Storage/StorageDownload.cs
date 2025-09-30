namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Options for storage download operations
/// </summary>
public class StorageDownloadOptions
{
    /// <summary>
    /// Specific version to download (if versioning enabled)
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Byte range to download (for partial downloads)
    /// </summary>
    public ByteRange? Range { get; set; }

    /// <summary>
    /// Whether to decrypt the file if encrypted
    /// </summary>
    public bool Decrypt { get; set; } = true;

    /// <summary>
    /// Conditional download based on ETag
    /// </summary>
    public string? IfMatch { get; set; }

    /// <summary>
    /// Conditional download based on modification time
    /// </summary>
    public DateTime? IfModifiedSince { get; set; }
}

/// <summary>
/// Result of storage download operation
/// </summary>
public class StorageDownloadResult : IDisposable
{
    /// <summary>
    /// File content stream
    /// </summary>
    public Stream Content { get; set; } = Stream.Null;

    /// <summary>
    /// File metadata
    /// </summary>
    public StorageFileInfo FileInfo { get; set; } = new();

    public string ContentType { get; set; } = string.Empty;
    public long ContentLength { get; set; }

    /// <summary>
    /// ETag for caching
    /// </summary>
    public string? ETag { get; set; }

    public DateTime LastModified { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public void Dispose()
    {
        Content?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Byte range for partial downloads
/// </summary>
public class ByteRange
{
    public long Start { get; set; }

    /// <summary>
    /// End byte position (inclusive)
    /// </summary>
    public long End { get; set; }

    public long? ContentLength { get; set; }
}
namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Result of signed URL generation
/// </summary>
public class StorageSignedUrlResult
{
    /// <summary>
    /// Generated signed URL
    /// </summary>
    public string SignedUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL expiration time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Permissions granted by this URL
    /// </summary>
    public StoragePermissions Permissions { get; set; }

    /// <summary>
    /// Whether URL generation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if generation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of permission update operation
/// </summary>
public class StoragePermissionResult
{
    /// <summary>
    /// File identifier
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Updated access control settings
    /// </summary>
    public StorageAccessControl AccessControl { get; set; } = new();

    /// <summary>
    /// Whether permission update was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Error message if update failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
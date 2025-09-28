namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Access control settings for storage files
/// </summary>
public class StorageAccessControl
{
    /// <summary>
    /// Public access level
    /// </summary>
    public PublicAccessLevel PublicAccess { get; set; } = PublicAccessLevel.Private;

    /// <summary>
    /// User-specific permissions
    /// </summary>
    public Dictionary<string, StoragePermissions> UserPermissions { get; set; } = new();

    /// <summary>
    /// Role-based permissions
    /// </summary>
    public Dictionary<string, StoragePermissions> RolePermissions { get; set; } = new();

    /// <summary>
    /// IP address restrictions
    /// </summary>
    public string[] AllowedIpRanges { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Expiration time for access
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Encryption options for storage
/// </summary>
public class EncryptionOptions
{
    /// <summary>
    /// Encryption method
    /// </summary>
    public EncryptionMethod Method { get; set; } = EncryptionMethod.ServerSide;

    /// <summary>
    /// Encryption key ID (for customer-managed keys)
    /// </summary>
    public string? KeyId { get; set; }

    /// <summary>
    /// Encryption algorithm
    /// </summary>
    public string Algorithm { get; set; } = "AES256";
}
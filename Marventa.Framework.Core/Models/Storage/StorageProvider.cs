namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Storage provider information
/// </summary>
public class StorageProviderInfo
{
    /// <summary>
    /// Provider name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Provider version
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Supported features
    /// </summary>
    public StorageFeatures SupportedFeatures { get; set; } = new();

    /// <summary>
    /// Provider-specific configuration
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new();

    /// <summary>
    /// Geographic regions available
    /// </summary>
    public string[] AvailableRegions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Maximum file size supported
    /// </summary>
    public long MaxFileSizeBytes { get; set; }

    /// <summary>
    /// Supported storage classes
    /// </summary>
    public StorageClass[] SupportedStorageClasses { get; set; } = Array.Empty<StorageClass>();
}

/// <summary>
/// Storage provider feature flags
/// </summary>
public class StorageFeatures
{
    /// <summary>
    /// Supports versioning
    /// </summary>
    public bool SupportsVersioning { get; set; }

    /// <summary>
    /// Supports server-side encryption
    /// </summary>
    public bool SupportsEncryption { get; set; }

    /// <summary>
    /// Supports access control lists
    /// </summary>
    public bool SupportsACL { get; set; }

    /// <summary>
    /// Supports signed URLs
    /// </summary>
    public bool SupportsSignedUrls { get; set; }

    /// <summary>
    /// Supports bulk operations
    /// </summary>
    public bool SupportsBulkOperations { get; set; }

    /// <summary>
    /// Supports partial downloads
    /// </summary>
    public bool SupportsRangeRequests { get; set; }

    /// <summary>
    /// Supports lifecycle management
    /// </summary>
    public bool SupportsLifecycleManagement { get; set; }

    /// <summary>
    /// Supports cross-region replication
    /// </summary>
    public bool SupportsReplication { get; set; }

    /// <summary>
    /// Supports content delivery network
    /// </summary>
    public bool SupportsCDN { get; set; }

    /// <summary>
    /// Supports analytics and metrics
    /// </summary>
    public bool SupportsAnalytics { get; set; }
}
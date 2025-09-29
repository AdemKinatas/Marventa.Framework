namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// Configuration options for storage services
/// </summary>
public class StorageOptions
{
    /// <summary>
    /// Storage provider name (LocalFile or Cloud)
    /// </summary>
    public string Provider { get; set; } = "LocalFile";

    /// <summary>
    /// Connection string for storage provider
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Base path for file storage
    /// </summary>
    public string? BasePath { get; set; } = "uploads";
}
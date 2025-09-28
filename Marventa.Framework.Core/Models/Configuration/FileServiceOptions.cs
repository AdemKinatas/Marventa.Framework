using Marventa.Framework.Core.Models.FileProcessing;

namespace Marventa.Framework.Core.Models.Configuration;

/// <summary>
/// Configuration options for file services
/// </summary>
public class FileServiceOptions
{
    /// <summary>
    /// Whether to enable file processor service
    /// </summary>
    public bool EnableFileProcessor { get; set; } = true;

    /// <summary>
    /// Whether to enable storage service
    /// </summary>
    public bool EnableStorage { get; set; } = true;

    /// <summary>
    /// Whether to enable CDN service
    /// </summary>
    public bool EnableCDN { get; set; } = false;

    /// <summary>
    /// Whether to enable ML service
    /// </summary>
    public bool EnableML { get; set; } = false;

    /// <summary>
    /// Whether to enable metadata service
    /// </summary>
    public bool EnableMetadata { get; set; } = true;

    /// <summary>
    /// File processor configuration
    /// </summary>
    public FileProcessorOptions FileProcessorOptions { get; set; } = new();

    /// <summary>
    /// Storage service configuration
    /// </summary>
    public StorageServiceOptions StorageOptions { get; set; } = new();

    /// <summary>
    /// CDN service configuration
    /// </summary>
    public CDNServiceOptions CDNOptions { get; set; } = new();

    /// <summary>
    /// ML service configuration
    /// </summary>
    public MLServiceOptions MLOptions { get; set; } = new();

    /// <summary>
    /// Metadata service configuration
    /// </summary>
    public MetadataServiceOptions MetadataOptions { get; set; } = new();
}

/// <summary>
/// Configuration options for file processor service
/// </summary>
public class FileProcessorOptions
{
    /// <summary>
    /// File processor provider to use
    /// </summary>
    public FileProcessorProvider Provider { get; set; } = FileProcessorProvider.ImageSharp;

    /// <summary>
    /// Maximum file size to process in bytes
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 50 * 1024 * 1024; // 50MB

    /// <summary>
    /// Supported image formats
    /// </summary>
    public string[] SupportedImageFormats { get; set; } = { "jpg", "jpeg", "png", "webp", "gif", "bmp" };

    /// <summary>
    /// Supported video formats
    /// </summary>
    public string[] SupportedVideoFormats { get; set; } = { "mp4", "avi", "mov", "wmv", "webm" };

    /// <summary>
    /// Default image quality for compression (1-100)
    /// </summary>
    public int DefaultImageQuality { get; set; } = 85;

    /// <summary>
    /// Default thumbnail sizes
    /// </summary>
    public ThumbnailSize[] DefaultThumbnailSizes { get; set; } =
    {
        new() { Name = "small", Width = 150, Height = 150 },
        new() { Name = "medium", Width = 300, Height = 300 },
        new() { Name = "large", Width = 600, Height = 600 }
    };

    /// <summary>
    /// Whether to preserve metadata during processing
    /// </summary>
    public bool PreserveMetadata { get; set; } = true;

    /// <summary>
    /// Temporary directory for processing
    /// </summary>
    public string? TempDirectory { get; set; }
}

/// <summary>
/// Configuration options for storage service
/// </summary>
public class StorageServiceOptions
{
    /// <summary>
    /// Storage provider to use
    /// </summary>
    public StorageProvider Provider { get; set; } = StorageProvider.LocalFile;

    /// <summary>
    /// Connection string for the storage provider
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Default container/bucket name
    /// </summary>
    public string DefaultContainer { get; set; } = "files";

    /// <summary>
    /// Base URL for public file access
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Default storage class
    /// </summary>
    public string DefaultStorageClass { get; set; } = "Standard";

    /// <summary>
    /// Whether to enable encryption by default
    /// </summary>
    public bool EnableEncryption { get; set; } = true;

    /// <summary>
    /// Whether to enable versioning by default
    /// </summary>
    public bool EnableVersioning { get; set; } = false;

    /// <summary>
    /// Maximum file size in bytes
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100MB

    /// <summary>
    /// Request timeout
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Retry policy settings
    /// </summary>
    public RetryPolicyOptions RetryPolicy { get; set; } = new();
}

/// <summary>
/// Configuration options for CDN service
/// </summary>
public class CDNServiceOptions
{
    /// <summary>
    /// CDN provider to use
    /// </summary>
    public CDNProvider Provider { get; set; } = CDNProvider.Mock;

    /// <summary>
    /// CDN endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for CDN provider
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Zone ID (for CloudFlare)
    /// </summary>
    public string? ZoneId { get; set; }

    /// <summary>
    /// Default cache TTL in seconds
    /// </summary>
    public int DefaultCacheTTL { get; set; } = 86400; // 24 hours

    /// <summary>
    /// Default browser cache TTL in seconds
    /// </summary>
    public int DefaultBrowserCacheTTL { get; set; } = 3600; // 1 hour

    /// <summary>
    /// Whether to enable compression
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Priority regions for content distribution
    /// </summary>
    public string[] PriorityRegions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Custom domain for CDN
    /// </summary>
    public string? CustomDomain { get; set; }
}

/// <summary>
/// Configuration options for ML service
/// </summary>
public class MLServiceOptions
{
    /// <summary>
    /// ML provider to use
    /// </summary>
    public MLProvider Provider { get; set; } = MLProvider.Mock;

    /// <summary>
    /// API endpoint for ML service
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for ML service
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Default confidence threshold
    /// </summary>
    public double DefaultConfidenceThreshold { get; set; } = 0.7;

    /// <summary>
    /// Maximum number of tags to generate
    /// </summary>
    public int MaxTagsPerImage { get; set; } = 10;

    /// <summary>
    /// Whether to enable content moderation
    /// </summary>
    public bool EnableContentModeration { get; set; } = true;

    /// <summary>
    /// Whether to enable sentiment analysis
    /// </summary>
    public bool EnableSentimentAnalysis { get; set; } = true;

    /// <summary>
    /// Request timeout
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Supported languages for text analysis
    /// </summary>
    public string[] SupportedLanguages { get; set; } = { "en", "es", "fr", "de", "it", "pt", "tr" };
}

/// <summary>
/// Configuration options for metadata service
/// </summary>
public class MetadataServiceOptions
{
    /// <summary>
    /// Metadata provider to use
    /// </summary>
    public MetadataProvider Provider { get; set; } = MetadataProvider.EntityFramework;

    /// <summary>
    /// Connection string for metadata storage
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Whether to enable full-text search
    /// </summary>
    public bool EnableFullTextSearch { get; set; } = true;

    /// <summary>
    /// Whether to enable similarity search
    /// </summary>
    public bool EnableSimilaritySearch { get; set; } = false;

    /// <summary>
    /// Whether to enable analytics
    /// </summary>
    public bool EnableAnalytics { get; set; } = true;

    /// <summary>
    /// Maximum number of tags per file
    /// </summary>
    public int MaxTagsPerFile { get; set; } = 50;

    /// <summary>
    /// Default similarity threshold
    /// </summary>
    public double DefaultSimilarityThreshold { get; set; } = 0.7;

    /// <summary>
    /// Search result page size
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Cache settings for metadata
    /// </summary>
    public MetadataCacheOptions CacheOptions { get; set; } = new();
}

/// <summary>
/// Retry policy configuration
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Base delay between retries
    /// </summary>
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Maximum delay between retries
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Whether to use exponential backoff
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;
}

/// <summary>
/// Cache options for metadata
/// </summary>
public class MetadataCacheOptions
{
    /// <summary>
    /// Whether to enable caching
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Default cache TTL
    /// </summary>
    public TimeSpan DefaultTTL { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Cache TTL for search results
    /// </summary>
    public TimeSpan SearchResultTTL { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Cache TTL for analytics
    /// </summary>
    public TimeSpan AnalyticsTTL { get; set; } = TimeSpan.FromHours(1);
}

/// <summary>
/// File processor provider types
/// </summary>
public enum FileProcessorProvider
{
    ImageSharp,
    Mock
}

/// <summary>
/// Storage provider types
/// </summary>
public enum StorageProvider
{
    LocalFile,
    AzureBlob,
    MinIO,
    Mock
}

/// <summary>
/// CDN provider types
/// </summary>
public enum CDNProvider
{
    CloudFlare,
    AzureCDN,
    Mock
}

/// <summary>
/// ML provider types
/// </summary>
public enum MLProvider
{
    AzureCognitive,
    OpenAI,
    Local,
    Mock
}

/// <summary>
/// Metadata provider types
/// </summary>
public enum MetadataProvider
{
    EntityFramework,
    MongoDB,
    Elasticsearch,
    Mock
}
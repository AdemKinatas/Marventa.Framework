using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Storage analytics and usage statistics
/// </summary>
public class StorageAnalyticsResult
{
    /// <summary>
    /// Time range for these analytics
    /// </summary>
    public TimeRange TimeRange { get; set; } = new();

    public long TotalStorageBytes { get; set; }
    public long TotalFileCount { get; set; }

    /// <summary>
    /// Storage usage by storage class
    /// </summary>
    public Dictionary<StorageClass, long> StorageByClass { get; set; } = new();

    /// <summary>
    /// Storage usage by content type
    /// </summary>
    public Dictionary<string, long> StorageByContentType { get; set; } = new();

    /// <summary>
    /// Storage growth over time
    /// </summary>
    public Dictionary<DateTime, long> StorageGrowth { get; set; } = new();

    /// <summary>
    /// API operation statistics
    /// </summary>
    public Dictionary<string, long> OperationCounts { get; set; } = new();

    /// <summary>
    /// Bandwidth usage statistics
    /// </summary>
    public BandwidthStatistics BandwidthStats { get; set; } = new();

    /// <summary>
    /// Cost breakdown (if available)
    /// </summary>
    public CostAnalysis? CostAnalysis { get; set; }

    /// <summary>
    /// Top accessed files
    /// </summary>
    public FileAccessStat[] TopAccessedFiles { get; set; } = Array.Empty<FileAccessStat>();
}

/// <summary>
/// Bandwidth usage statistics
/// </summary>
public class BandwidthStatistics
{
    public long TotalUploadBytes { get; set; }
    public long TotalDownloadBytes { get; set; }

    /// <summary>
    /// Upload bandwidth by time period
    /// </summary>
    public Dictionary<DateTime, long> UploadByTime { get; set; } = new();

    /// <summary>
    /// Download bandwidth by time period
    /// </summary>
    public Dictionary<DateTime, long> DownloadByTime { get; set; } = new();

    /// <summary>
    /// Peak upload rate in bytes per second
    /// </summary>
    public double PeakUploadRate { get; set; }

    /// <summary>
    /// Peak download rate in bytes per second
    /// </summary>
    public double PeakDownloadRate { get; set; }
}

/// <summary>
/// Cost analysis information
/// </summary>
public class CostAnalysis
{
    /// <summary>
    /// Total cost in the specified currency
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Cost breakdown by category
    /// </summary>
    public Dictionary<string, decimal> CostByCategory { get; set; } = new();

    /// <summary>
    /// Cost by storage class
    /// </summary>
    public Dictionary<StorageClass, decimal> CostByStorageClass { get; set; } = new();

    /// <summary>
    /// Projected monthly cost
    /// </summary>
    public decimal ProjectedMonthlyCost { get; set; }
}

/// <summary>
/// File access statistics
/// </summary>
public class FileAccessStat
{
    /// <summary>
    /// File identifier
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Number of accesses
    /// </summary>
    public long AccessCount { get; set; }

    /// <summary>
    /// Total bytes transferred
    /// </summary>
    public long BytesTransferred { get; set; }

    /// <summary>
    /// Last access time
    /// </summary>
    public DateTime LastAccessed { get; set; }
}
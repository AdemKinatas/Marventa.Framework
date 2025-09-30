using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Models.FileMetadata;

/// <summary>
/// Result of file analytics analysis
/// </summary>
public class FileAnalyticsResult
{
    /// <summary>
    /// Whether the analysis was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the analysis failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that was analyzed (null for global analytics)
    /// </summary>
    public string? FileId { get; set; }

    /// <summary>
    /// Time range that was analyzed
    /// </summary>
    public TimeRange? TimeRange { get; set; }

    /// <summary>
    /// Total number of accesses
    /// </summary>
    public long TotalAccesses { get; set; }

    /// <summary>
    /// Unique users who accessed the file(s)
    /// </summary>
    public int UniqueUsers { get; set; }

    /// <summary>
    /// Access patterns by type
    /// </summary>
    public Dictionary<FileAccessType, long> AccessByType { get; set; } = new();

    /// <summary>
    /// Access patterns over time
    /// </summary>
    public Dictionary<DateTime, long> AccessOverTime { get; set; } = new();

    /// <summary>
    /// Peak access time
    /// </summary>
    public DateTime? PeakAccessTime { get; set; }

    /// <summary>
    /// Most active users
    /// </summary>
    public List<UserAccessStatistics> MostActiveUsers { get; set; } = new();

    /// <summary>
    /// Access patterns by geographic location (if available)
    /// </summary>
    public Dictionary<string, long> AccessByLocation { get; set; } = new();

    /// <summary>
    /// Average time between accesses
    /// </summary>
    public TimeSpan? AverageTimeBetweenAccesses { get; set; }

    /// <summary>
    /// Total number of files in analytics
    /// </summary>
    public long TotalFiles { get; set; }

    /// <summary>
    /// Total number of views across all files
    /// </summary>
    public long TotalViews { get; set; }

    /// <summary>
    /// Total number of downloads across all files
    /// </summary>
    public long TotalDownloads { get; set; }

    /// <summary>
    /// Average rating across all files
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Most frequently used tags
    /// </summary>
    public List<string> TopTags { get; set; } = new();

    /// <summary>
    /// Distribution of files by type
    /// </summary>
    public Dictionary<string, long> FileTypeDistribution { get; set; } = new();

    /// <summary>
    /// Time taken for the analysis
    /// </summary>
    public TimeSpan AnalysisTime { get; set; }
}

/// <summary>
/// Result of access recording operation
/// </summary>
public class AccessRecordingResult
{
    /// <summary>
    /// Whether the recording was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the recording failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// File ID that access was recorded for
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for this access record
    /// </summary>
    public string AccessId { get; set; } = string.Empty;

    /// <summary>
    /// Type of access that was recorded
    /// </summary>
    public FileAccessType AccessType { get; set; }

    /// <summary>
    /// User who accessed the file
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Timestamp when access was recorded
    /// </summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>
    /// Total access count for this file after recording
    /// </summary>
    public long TotalAccessCount { get; set; }

    /// <summary>
    /// Time taken for the recording operation
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// User access statistics
/// </summary>
public class UserAccessStatistics
{
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Total number of accesses by this user
    /// </summary>
    public long AccessCount { get; set; }

    /// <summary>
    /// Last access time
    /// </summary>
    public DateTime LastAccess { get; set; }

    /// <summary>
    /// Access breakdown by type
    /// </summary>
    public Dictionary<FileAccessType, long> AccessByType { get; set; } = new();

    /// <summary>
    /// Most accessed files by this user
    /// </summary>
    public List<string> TopAccessedFiles { get; set; } = new();
}

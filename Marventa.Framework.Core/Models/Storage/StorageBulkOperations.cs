namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Options for bulk storage operations
/// </summary>
public class StorageBulkOptions
{
    /// <summary>
    /// Maximum number of parallel operations
    /// </summary>
    public int MaxParallelism { get; set; } = 5;

    /// <summary>
    /// Whether to continue on individual failures
    /// </summary>
    public bool ContinueOnFailure { get; set; } = true;

    /// <summary>
    /// Progress callback for bulk operations
    /// </summary>
    public IProgress<BulkOperationProgress>? Progress { get; set; }

    /// <summary>
    /// Timeout for individual operations
    /// </summary>
    public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromMinutes(5);
}

/// <summary>
/// Result of bulk upload operation
/// </summary>
public class StorageBulkUploadResult
{
    public int TotalFiles { get; set; }
    public int SuccessfulUploads { get; set; }
    public int FailedUploads { get; set; }

    /// <summary>
    /// Results for each file
    /// </summary>
    public Dictionary<string, StorageUploadResult> Results { get; set; } = new();

    /// <summary>
    /// Overall operation success rate
    /// </summary>
    public double SuccessRate => TotalFiles > 0 ? (double)SuccessfulUploads / TotalFiles : 0;

    public long TotalBytesUploaded { get; set; }
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Average upload speed in bytes per second
    /// </summary>
    public double AverageSpeedBytesPerSecond => Duration.TotalSeconds > 0 ? TotalBytesUploaded / Duration.TotalSeconds : 0;
}

/// <summary>
/// Result of bulk deletion operation
/// </summary>
public class StorageBulkDeletionResult
{
    public int TotalFiles { get; set; }
    public int SuccessfulDeletions { get; set; }
    public int FailedDeletions { get; set; }

    /// <summary>
    /// Results for each file
    /// </summary>
    public Dictionary<string, StorageDeletionResult> Results { get; set; } = new();

    /// <summary>
    /// Overall operation success rate
    /// </summary>
    public double SuccessRate => TotalFiles > 0 ? (double)SuccessfulDeletions / TotalFiles : 0;

    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Progress information for bulk operations
/// </summary>
public class BulkOperationProgress
{
    public int TotalItems { get; set; }
    public int CompletedItems { get; set; }
    public int FailedItems { get; set; }
    public string CurrentOperation { get; set; } = string.Empty;

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public double ProgressPercentage => TotalItems > 0 ? (double)CompletedItems / TotalItems * 100 : 0;

    /// <summary>
    /// Estimated time remaining
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining { get; set; }
}
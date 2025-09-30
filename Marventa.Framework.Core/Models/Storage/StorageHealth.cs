namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Storage provider health information
/// </summary>
public class StorageHealthResult
{
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Response time to storage provider
    /// </summary>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Available storage capacity (if known)
    /// </summary>
    public long? AvailableCapacityBytes { get; set; }

    public long UsedCapacityBytes { get; set; }
    public DateTime CheckedAt { get; set; }

    /// <summary>
    /// Detailed health information
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// Error messages if health check failed
    /// </summary>
    public string[] Errors { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Storage connectivity test result
/// </summary>
public class StorageConnectivityResult
{
    public bool Success { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public DateTime TestedAt { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional test details
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();
}
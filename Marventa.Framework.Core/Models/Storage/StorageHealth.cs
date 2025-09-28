namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Storage provider health information
/// </summary>
public class StorageHealthResult
{
    /// <summary>
    /// Overall health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Response time to storage provider
    /// </summary>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Available storage capacity (if known)
    /// </summary>
    public long? AvailableCapacityBytes { get; set; }

    /// <summary>
    /// Used storage capacity
    /// </summary>
    public long UsedCapacityBytes { get; set; }

    /// <summary>
    /// Health check timestamp
    /// </summary>
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
    /// <summary>
    /// Whether connection test was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Connection response time
    /// </summary>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Test timestamp
    /// </summary>
    public DateTime TestedAt { get; set; }

    /// <summary>
    /// Provider endpoint that was tested
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Error message if connection failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional test details
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();
}
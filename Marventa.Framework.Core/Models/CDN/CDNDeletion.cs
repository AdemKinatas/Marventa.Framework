namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Result of CDN deletion operation
/// </summary>
public class CDNDeletionResult
{
    /// <summary>
    /// Deletion request ID
    /// </summary>
    public string DeletionId { get; set; } = string.Empty;

    /// <summary>
    /// URLs that were deleted
    /// </summary>
    public string[] DeletedUrls { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Number of edge locations cleared
    /// </summary>
    public int EdgeLocationsClearedCount { get; set; }

    /// <summary>
    /// Estimated completion time
    /// </summary>
    public DateTime EstimatedCompletionTime { get; set; }

    /// <summary>
    /// Whether deletion was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if deletion failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// Result of CDN deletion operation
/// </summary>
public class CDNDeletionResult
{
    public string DeletionId { get; set; } = string.Empty;

    /// <summary>
    /// URLs that were deleted
    /// </summary>
    public string[] DeletedUrls { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Number of edge locations cleared
    /// </summary>
    public int EdgeLocationsClearedCount { get; set; }

    public DateTime EstimatedCompletionTime { get; set; }

    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
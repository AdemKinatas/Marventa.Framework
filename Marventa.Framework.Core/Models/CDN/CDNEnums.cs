namespace Marventa.Framework.Core.Models.CDN;

/// <summary>
/// CDN access levels
/// </summary>
public enum CDNAccessLevel
{
    Public,
    Private,
    Authenticated
}

/// <summary>
/// Invalidation status
/// </summary>
public enum InvalidationStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

/// <summary>
/// Warmup status
/// </summary>
public enum WarmupStatus
{
    Queued,
    InProgress,
    Completed,
    Failed
}

/// <summary>
/// Distribution state
/// </summary>
public enum DistributionState
{
    Uploading,
    Propagating,
    Distributed,
    Failed,
    Removed
}

/// <summary>
/// Edge location status
/// </summary>
public enum EdgeStatus
{
    Syncing,
    Available,
    Unavailable,
    Error
}
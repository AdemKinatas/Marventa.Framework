namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// Rate limiting configuration settings
/// </summary>
public class RateLimitConfig
{
    /// <summary>
    /// Maximum number of requests allowed within the time window
    /// </summary>
    public int MaxRequests { get; set; } = 100;

    /// <summary>
    /// Time window in minutes
    /// </summary>
    public int WindowMinutes { get; set; } = 15;

    /// <summary>
    /// Enable per-user rate limiting
    /// </summary>
    public bool EnablePerUser { get; set; } = true;

    /// <summary>
    /// Enable per-IP rate limiting
    /// </summary>
    public bool EnablePerIP { get; set; } = true;
}
namespace Marventa.Framework.Web.Models;

/// <summary>
/// Configuration options for rate limiting middleware
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Maximum number of requests allowed within the time window
    /// </summary>
    public int MaxRequests { get; set; } = 100;

    /// <summary>
    /// Time window for rate limiting
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(15);
}
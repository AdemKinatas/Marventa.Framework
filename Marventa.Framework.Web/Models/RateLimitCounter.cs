namespace Marventa.Framework.Web.Models;

/// <summary>
/// Internal rate limit counter for tracking request counts and reset times
/// </summary>
internal class RateLimitCounter
{
    /// <summary>
    /// Current number of requests within the time window
    /// </summary>
    public int RequestCount { get; set; }

    /// <summary>
    /// When the current window resets
    /// </summary>
    public DateTime ResetTime { get; set; }
}
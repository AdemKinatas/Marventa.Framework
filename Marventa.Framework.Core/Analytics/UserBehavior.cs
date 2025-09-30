namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents aggregated user behavior analytics including page views, events, and engagement patterns.
/// Used for user segmentation and behavioral analysis.
/// </summary>
public class UserBehavior
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of page views by this user.
    /// </summary>
    public int PageViews { get; set; }

    /// <summary>
    /// Gets or sets the total number of events triggered by this user.
    /// </summary>
    public int Events { get; set; }

    /// <summary>
    /// Gets or sets the average session duration for this user across all sessions.
    /// </summary>
    public TimeSpan AverageSessionDuration { get; set; }

    /// <summary>
    /// Gets or sets the list of most frequently visited pages by this user.
    /// </summary>
    public List<string> TopPages { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of most frequently triggered events by this user.
    /// </summary>
    public List<string> TopEvents { get; set; } = new();

    /// <summary>
    /// Gets or sets the count of each event type triggered by this user.
    /// Keys are event names, values are occurrence counts.
    /// </summary>
    public Dictionary<string, int> EventCounts { get; set; } = new();
}
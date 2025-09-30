namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents a page view event tracking user navigation through the application.
/// Captures URL, duration, referrer, and user context for web analytics.
/// </summary>
public class PageView
{
    /// <summary>
    /// Gets or sets the URL of the viewed page.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the page.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who viewed this page.
    /// Null for anonymous users.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the session identifier for grouping page views in the same session.
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier for multi-tenant applications.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the referrer URL (the page the user came from).
    /// </summary>
    public string? Referrer { get; set; }

    /// <summary>
    /// Gets or sets the time the user spent on this page.
    /// Null if duration tracking is not available.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when this page view occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
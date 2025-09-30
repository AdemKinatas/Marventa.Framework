namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents an analytics event that tracks user actions and behavior in the application.
/// Events can contain custom properties and metrics for detailed analysis.
/// </summary>
public class AnalyticsEvent
{
    /// <summary>
    /// Gets or sets the name of the event (e.g., "PageView", "ButtonClick", "Purchase").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the user who triggered this event.
    /// Null if the user is anonymous or not authenticated.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the session identifier for grouping events within the same user session.
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier for multi-tenant applications.
    /// Null for non-tenant-specific events.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets custom properties associated with this event (e.g., page URL, button ID).
    /// Properties are stored as key-value pairs for flexible event metadata.
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Gets or sets numeric metrics associated with this event (e.g., response time, amount).
    /// Metrics can be aggregated for analytics dashboards.
    /// </summary>
    public Dictionary<string, double> Metrics { get; set; } = new();

    /// <summary>
    /// Gets or sets the UTC timestamp when this event occurred.
    /// Defaults to current UTC time when the event is created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
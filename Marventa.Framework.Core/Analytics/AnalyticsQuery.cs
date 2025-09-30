namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents a query for retrieving and filtering analytics data.
/// Supports date ranges, grouping, and custom filters for flexible reporting.
/// </summary>
public class AnalyticsQuery
{
    /// <summary>
    /// Gets or sets the name of the event to filter by.
    /// Null to include all events.
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// Gets or sets the start date for the query range (inclusive).
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date for the query range (inclusive).
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the list of fields to group results by (e.g., "UserId", "EventName").
    /// Empty list means no grouping.
    /// </summary>
    public List<string> GroupBy { get; set; } = new();

    /// <summary>
    /// Gets or sets custom filters to apply to the query.
    /// Filters are key-value pairs matching event properties.
    /// </summary>
    public Dictionary<string, object> Filters { get; set; } = new();

    /// <summary>
    /// Gets or sets the tenant identifier to filter by for multi-tenant applications.
    /// Null to include all tenants (admin queries only).
    /// </summary>
    public string? TenantId { get; set; }
}
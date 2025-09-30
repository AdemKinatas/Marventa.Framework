namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents the result of an analytics query containing data points, totals, and metadata.
/// Used for generating analytics dashboards and reports.
/// </summary>
public class AnalyticsReport
{
    /// <summary>
    /// Gets or sets the list of data points in the report.
    /// Each data point represents aggregated data for a specific time period or grouping.
    /// </summary>
    public List<DataPoint> DataPoints { get; set; } = new();

    /// <summary>
    /// Gets or sets the total aggregated values across all data points.
    /// Keys are metric names, values are the totals.
    /// </summary>
    public Dictionary<string, double> Totals { get; set; } = new();

    /// <summary>
    /// Gets or sets additional metadata about the report (e.g., query parameters, generation time).
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
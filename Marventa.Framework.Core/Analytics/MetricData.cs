namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents a time-series metric data point with tags for categorization.
/// Used for performance monitoring and operational metrics.
/// </summary>
public class MetricData
{
    /// <summary>
    /// Gets or sets the name of the metric (e.g., "ResponseTime", "ErrorRate").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the numeric value of the metric.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when this metric was recorded.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets tags for categorizing and filtering metrics (e.g., "Environment", "Region").
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new();
}
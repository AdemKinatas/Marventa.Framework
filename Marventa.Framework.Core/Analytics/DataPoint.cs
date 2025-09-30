namespace Marventa.Framework.Core.Analytics;

/// <summary>
/// Represents a single data point in an analytics report with timestamp and aggregated values.
/// </summary>
public class DataPoint
{
    /// <summary>
    /// Gets or sets the timestamp for this data point.
    /// Typically represents the start of a time bucket for aggregated data.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the aggregated values for this data point.
    /// Keys are metric or dimension names, values are the aggregated results.
    /// </summary>
    public Dictionary<string, object> Values { get; set; } = new();
}
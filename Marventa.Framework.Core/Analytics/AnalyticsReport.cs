namespace Marventa.Framework.Core.Analytics;

public class AnalyticsReport
{
    public List<DataPoint> DataPoints { get; set; } = new();
    public Dictionary<string, double> Totals { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}
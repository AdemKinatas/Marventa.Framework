namespace Marventa.Framework.Core.Analytics;

public class DataPoint
{
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Values { get; set; } = new();
}
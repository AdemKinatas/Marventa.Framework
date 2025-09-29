namespace Marventa.Framework.Core.Interfaces.Services;

public class AggregationRequest
{
    public string Field { get; set; } = string.Empty;
    public AggregationType Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}
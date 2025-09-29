namespace Marventa.Framework.Core.Interfaces.Services;

public class AggregationResult
{
    public object? Value { get; set; }
    public Dictionary<string, object> Buckets { get; set; } = new();
}
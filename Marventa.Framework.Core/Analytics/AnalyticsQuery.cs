namespace Marventa.Framework.Core.Analytics;

public class AnalyticsQuery
{
    public string? EventName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> GroupBy { get; set; } = new();
    public Dictionary<string, object> Filters { get; set; } = new();
    public string? TenantId { get; set; }
}
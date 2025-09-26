namespace Marventa.Framework.Core.Analytics;

public class AnalyticsEvent
{
    public string Name { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public Dictionary<string, double> Metrics { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
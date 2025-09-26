namespace Marventa.Framework.Core.Analytics;

public class PageView
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public string? Referrer { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
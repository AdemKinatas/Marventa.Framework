namespace Marventa.Framework.Core.Interfaces;

public interface IAnalyticsService
{
    Task TrackEventAsync(AnalyticsEvent analyticsEvent, CancellationToken cancellationToken = default);
    Task TrackMetricAsync(string name, double value, Dictionary<string, string>? properties = null, CancellationToken cancellationToken = default);
    Task TrackPageViewAsync(PageView pageView, CancellationToken cancellationToken = default);
    Task TrackExceptionAsync(Exception exception, Dictionary<string, string>? properties = null, CancellationToken cancellationToken = default);
    Task<AnalyticsReport> GetReportAsync(AnalyticsQuery query, CancellationToken cancellationToken = default);
    Task<List<MetricData>> GetMetricsAsync(string metricName, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<UserBehavior> GetUserBehaviorAsync(string userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

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

public class AnalyticsQuery
{
    public string? EventName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> GroupBy { get; set; } = new();
    public Dictionary<string, object> Filters { get; set; } = new();
    public string? TenantId { get; set; }
}

public class AnalyticsReport
{
    public List<DataPoint> DataPoints { get; set; } = new();
    public Dictionary<string, double> Totals { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class DataPoint
{
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Values { get; set; } = new();
}

public class MetricData
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Tags { get; set; } = new();
}

public class UserBehavior
{
    public string UserId { get; set; } = string.Empty;
    public int PageViews { get; set; }
    public int Events { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<string> TopPages { get; set; } = new();
    public List<string> TopEvents { get; set; } = new();
    public Dictionary<string, int> EventCounts { get; set; } = new();
}
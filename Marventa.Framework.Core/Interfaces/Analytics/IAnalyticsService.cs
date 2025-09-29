using Marventa.Framework.Core.Analytics;

namespace Marventa.Framework.Core.Interfaces.Analytics;

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
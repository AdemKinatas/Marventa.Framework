namespace Marventa.Framework.Core.Interfaces.HealthCheck;

public class HealthCheckResult
{
    public HealthStatus Status { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public TimeSpan Duration { get; set; }

    public static HealthCheckResult Healthy(string? description = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Healthy,
            Description = description,
            Data = data ?? new Dictionary<string, object>()
        };
    }

    public static HealthCheckResult Degraded(string? description = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Degraded,
            Description = description,
            Data = data ?? new Dictionary<string, object>()
        };
    }

    public static HealthCheckResult Unhealthy(string? description = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Unhealthy,
            Description = description,
            Data = data ?? new Dictionary<string, object>()
        };
    }
}
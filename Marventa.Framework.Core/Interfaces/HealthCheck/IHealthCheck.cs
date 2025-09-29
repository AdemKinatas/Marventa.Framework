namespace Marventa.Framework.Core.Interfaces.HealthCheck;

public interface IHealthCheck
{
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    string Name { get; }
}
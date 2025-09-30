using Marventa.Framework.EventBus.RabbitMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Marventa.Framework.HealthChecks;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnection _connection;

    public RabbitMqHealthCheck(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_connection.IsConnected)
            {
                return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ connection is healthy"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is not established"));
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is unhealthy", ex));
        }
    }
}

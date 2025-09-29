using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces.HealthCheck;
using Marventa.Framework.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public string Name => "Database";

    public DatabaseHealthCheck(IConnectionFactory connectionFactory, ILogger<DatabaseHealthCheck> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["connectionString"] = MaskConnectionString(_connectionFactory.GetConnectionString()),
                ["database"] = connection.Database
            };

            stopwatch.Stop();

            _logger.LogDebug("Database health check completed successfully in {Duration}ms", stopwatch.ElapsedMilliseconds);

            return new HealthCheckResult
            {
                Status = HealthStatus.Healthy,
                Description = "Database connection successful",
                Data = data,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Database health check failed");

            return new HealthCheckResult
            {
                Status = HealthStatus.Unhealthy,
                Description = $"Database connection failed: {ex.Message}",
                Duration = stopwatch.Elapsed
            };
        }
    }

    private static string MaskConnectionString(string connectionString)
    {
        // Simple masking for security
        if (connectionString.Contains("Password="))
        {
            var parts = connectionString.Split(';');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Contains("Password="))
                {
                    parts[i] = "Password=***";
                }
            }
            return string.Join(";", parts);
        }
        return connectionString;
    }
}
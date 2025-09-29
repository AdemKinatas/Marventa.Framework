using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces.HealthCheck;
using Marventa.Framework.Core.Interfaces.Caching;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.HealthChecks;

public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheHealthCheck> _logger;

    public string Name => "Cache";

    public CacheHealthCheck(ICacheService cacheService, ILogger<CacheHealthCheck> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var testKey = $"health_check_{Guid.NewGuid()}";
        var testValue = "health_check_value";

        try
        {
            // Test write
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromMinutes(1), cancellationToken);

            // Test read
            var retrievedValue = await _cacheService.GetAsync<string>(testKey, cancellationToken);

            // Test delete
            await _cacheService.RemoveAsync(testKey, cancellationToken);

            stopwatch.Stop();

            if (string.Equals(retrievedValue, testValue, StringComparison.Ordinal))
            {
                _logger.LogDebug("Cache health check completed successfully in {Duration}ms", stopwatch.ElapsedMilliseconds);

                return new HealthCheckResult
                {
                    Status = HealthStatus.Healthy,
                    Description = "Cache is working properly",
                    Data = new Dictionary<string, object>
                    {
                        ["responseTime"] = stopwatch.ElapsedMilliseconds
                    },
                    Duration = stopwatch.Elapsed
                };
            }
            else
            {
                _logger.LogWarning("Cache health check failed - retrieved value doesn't match");

                return new HealthCheckResult
                {
                    Status = HealthStatus.Degraded,
                    Description = "Cache read/write test failed",
                    Duration = stopwatch.Elapsed
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Cache health check failed");

            return new HealthCheckResult
            {
                Status = HealthStatus.Unhealthy,
                Description = $"Cache service failed: {ex.Message}",
                Duration = stopwatch.Elapsed
            };
        }
    }
}
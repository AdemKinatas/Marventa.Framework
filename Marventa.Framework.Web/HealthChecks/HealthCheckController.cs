using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Application.DTOs;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.HealthChecks;

[AllowAnonymous]
public class HealthController : BaseController
{
    private readonly IEnumerable<IHealthCheck> _healthChecks;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IEnumerable<IHealthCheck> healthChecks, ILogger<HealthController> logger)
    {
        _healthChecks = healthChecks;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<ActionResult<ApiResponse<HealthReport>>> GetHealth(CancellationToken cancellationToken = default)
    {
        var overallStopwatch = Stopwatch.StartNew();
        var healthResults = new List<HealthCheckEntry>();

        foreach (var healthCheck in _healthChecks)
        {
            try
            {
                var result = await healthCheck.CheckHealthAsync(cancellationToken);
                healthResults.Add(new HealthCheckEntry
                {
                    Name = healthCheck.Name,
                    Status = result.Status.ToString(),
                    Description = result.Description,
                    Duration = result.Duration,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check {HealthCheckName} threw an exception", healthCheck.Name);
                healthResults.Add(new HealthCheckEntry
                {
                    Name = healthCheck.Name,
                    Status = HealthStatus.Unhealthy.ToString(),
                    Description = $"Health check threw an exception: {ex.Message}",
                    Duration = TimeSpan.Zero,
                    Data = new Dictionary<string, object>()
                });
            }
        }

        overallStopwatch.Stop();

        var overallStatus = DetermineOverallStatus(healthResults);
        var healthReport = new HealthReport
        {
            Status = overallStatus.ToString(),
            TotalDuration = overallStopwatch.Elapsed,
            Entries = healthResults
        };

        if (overallStatus == HealthStatus.Healthy)
        {
            return Ok(healthReport, "All health checks passed");
        }
        else if (overallStatus == HealthStatus.Degraded)
        {
            Response.StatusCode = 200; // Still return 200 for degraded
            return Ok(healthReport, "Some health checks are degraded");
        }
        else
        {
            Response.StatusCode = 503; // Service Unavailable
            return Ok(healthReport, "Health checks failed");
        }
    }

    [HttpGet("health/ready")]
    public async Task<ActionResult<ApiResponse<string>>> GetReadiness(CancellationToken cancellationToken = default)
    {
        // Simple readiness check - you can customize this logic
        var criticalHealthChecks = _healthChecks.Where(hc => hc.Name == "Database" || hc.Name == "Cache");

        foreach (var healthCheck in criticalHealthChecks)
        {
            var result = await healthCheck.CheckHealthAsync(cancellationToken);
            if (result.Status == HealthStatus.Unhealthy)
            {
                Response.StatusCode = 503;
                return Ok("Not Ready", $"Critical service {healthCheck.Name} is unhealthy");
            }
        }

        return Ok("Ready", "Service is ready to accept requests");
    }

    [HttpGet("health/live")]
    public ActionResult<ApiResponse<string>> GetLiveness()
    {
        // Simple liveness check - service is running
        return Ok("Alive", "Service is alive");
    }

    private static HealthStatus DetermineOverallStatus(List<HealthCheckEntry> results)
    {
        if (results.Any(r => r.Status == HealthStatus.Unhealthy.ToString()))
            return HealthStatus.Unhealthy;

        if (results.Any(r => r.Status == HealthStatus.Degraded.ToString()))
            return HealthStatus.Degraded;

        return HealthStatus.Healthy;
    }
}
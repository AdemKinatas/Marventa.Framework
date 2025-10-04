using Marventa.Framework.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResilienceTestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ResilienceTestController> _logger;
    private static int _failureCount = 0;
    private static DateTime _lastFailureTime = DateTime.MinValue;

    public ResilienceTestController(IHttpClientFactory httpClientFactory, ILogger<ResilienceTestController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Simulates a failing service to test circuit breaker
    /// </summary>
    [HttpGet("simulate-failure")]
    public IActionResult SimulateFailure([FromQuery] bool shouldFail = true)
    {
        if (shouldFail)
        {
            Interlocked.Increment(ref _failureCount);
            _lastFailureTime = DateTime.UtcNow;
            _logger.LogWarning("Simulated failure #{FailureCount}", _failureCount);
            throw new InvalidOperationException($"Simulated failure (count: {_failureCount})");
        }

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Service is working",
                failureCount = _failureCount,
                lastFailureTime = _lastFailureTime == DateTime.MinValue ? (DateTime?)null : _lastFailureTime,
                timestamp = DateTime.UtcNow
            },
            "Service successful"
        );

        return Ok(response);
    }

    /// <summary>
    /// Simulates a slow service to test timeout policies
    /// </summary>
    [HttpGet("simulate-timeout")]
    public async Task<IActionResult> SimulateTimeout([FromQuery] int delayMs = 3000)
    {
        _logger.LogInformation("Simulating slow service with {DelayMs}ms delay", delayMs);

        await Task.Delay(delayMs);

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Slow service completed",
                delayMs,
                timestamp = DateTime.UtcNow
            },
            "Timeout simulation completed"
        );

        return Ok(response);
    }

    /// <summary>
    /// Demonstrates using a resilient HttpClient
    /// </summary>
    [HttpGet("call-external")]
    public async Task<IActionResult> CallExternalService([FromQuery] string url = "https://api.github.com")
    {
        try
        {
            var client = _httpClientFactory.CreateClient("resilient-client");
            client.Timeout = TimeSpan.FromSeconds(5);

            _logger.LogInformation("Calling external service: {Url}", url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = ApiResponse<object>.SuccessResponse(
                new
                {
                    url,
                    statusCode = (int)response.StatusCode,
                    contentLength = content.Length,
                    headers = response.Headers.Select(h => new { h.Key, Value = string.Join(", ", h.Value) }),
                    message = "External service call successful"
                },
                "Resilient HTTP call completed"
            );

            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call external service: {Url}", url);

            var errorResponse = ApiResponse<object>.ErrorResponse(
                $"External service call failed: {ex.Message}"
            );

            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Gets circuit breaker status
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                failureCount = _failureCount,
                lastFailureTime = _lastFailureTime == DateTime.MinValue ? (DateTime?)null : _lastFailureTime,
                timeSinceLastFailure = _lastFailureTime == DateTime.MinValue ? (TimeSpan?)null : DateTime.UtcNow - _lastFailureTime,
                message = "Circuit breaker policies can be configured using Polly extensions"
            },
            "Resilience status"
        );

        return Ok(response);
    }

    /// <summary>
    /// Reset failure counter
    /// </summary>
    [HttpPost("reset")]
    public IActionResult Reset()
    {
        var oldCount = _failureCount;
        _failureCount = 0;
        _lastFailureTime = DateTime.MinValue;

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                previousFailureCount = oldCount,
                currentFailureCount = _failureCount,
                message = "Failure counter reset"
            },
            "Reset successful"
        );

        return Ok(response);
    }
}

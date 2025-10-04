using Marventa.Framework.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoggingTestController : ControllerBase
{
    private readonly ILogger<LoggingTestController> _logger;

    public LoggingTestController(ILogger<LoggingTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple GET request to test request/response logging
    /// Check logs to see full request/response details
    /// </summary>
    [HttpGet("simple")]
    public IActionResult SimpleGet()
    {
        _logger.LogInformation("SimpleGet endpoint called");

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Check your logs to see this request and response logged",
                timestamp = DateTime.UtcNow,
                headers = Request.Headers.Select(h => new { h.Key, Value = string.Join(", ", h.Value.ToArray()) })
            },
            "Request/Response logging test"
        );

        return Ok(response);
    }

    /// <summary>
    /// POST request with sensitive data to test masking
    /// </summary>
    [HttpPost("sensitive-data")]
    public IActionResult PostSensitiveData([FromBody] SensitiveDataRequest request)
    {
        _logger.LogInformation("Processing request with sensitive data - check logs to verify masking");

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Sensitive data processed. Check logs - password/token/secret fields should be ***MASKED***",
                username = request.Username,
                passwordMasked = "***MASKED***",
                tokenMasked = "***MASKED***"
            },
            "Sensitive data handled securely"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test endpoint with API key to verify header masking
    /// Send header: X-API-Key: test-api-key-12345
    /// </summary>
    [HttpGet("with-api-key")]
    public IActionResult WithApiKey()
    {
        var apiKey = Request.Headers["X-API-Key"].ToString();

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Check logs - X-API-Key header should be ***MASKED***",
                apiKeyReceived = !string.IsNullOrEmpty(apiKey),
                timestamp = DateTime.UtcNow
            },
            "API key header masking test"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test endpoint with authorization header
    /// Send header: Authorization: Bearer some-jwt-token
    /// </summary>
    [HttpGet("with-auth")]
    public IActionResult WithAuthorization()
    {
        var authHeader = Request.Headers["Authorization"].ToString();

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Check logs - Authorization header should be ***MASKED***",
                authHeaderReceived = !string.IsNullOrEmpty(authHeader),
                timestamp = DateTime.UtcNow
            },
            "Authorization header masking test"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test endpoint with large response body
    /// </summary>
    [HttpGet("large-response")]
    public IActionResult LargeResponse()
    {
        var largeData = Enumerable.Range(1, 100).Select(i => new
        {
            Id = i,
            Name = $"Item {i}",
            Description = $"This is a description for item {i} with some additional text to make it larger",
            Data = string.Join("", Enumerable.Range(1, 50).Select(j => $"data-{j}"))
        });

        var response = ApiResponse<object>.SuccessResponse(
            largeData,
            "Large response - check logs to see if it's truncated based on MaxBodyLogSize setting"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test different content types
    /// </summary>
    [HttpPost("xml-data")]
    [Consumes("application/xml")]
    public IActionResult XmlData()
    {
        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "XML content type test - check logs to see if XML is logged",
                timestamp = DateTime.UtcNow
            },
            "XML content logging test"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test multipart form data
    /// </summary>
    [HttpPost("upload")]
    public IActionResult Upload(IFormFile? file)
    {
        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "File upload test - multipart/form-data should not be logged in body",
                fileReceived = file != null,
                fileName = file?.FileName,
                fileSize = file?.Length,
                timestamp = DateTime.UtcNow
            },
            "File upload logging test"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test query string logging
    /// </summary>
    [HttpGet("with-querystring")]
    public IActionResult WithQueryString([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] string? apikey = null)
    {
        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Query string test - check logs to see full URL with query parameters",
                search,
                page,
                apikeyMasked = apikey != null ? "***MASKED***" : null,
                timestamp = DateTime.UtcNow
            },
            "Query string logging test"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test slow request to verify elapsed time logging
    /// </summary>
    [HttpGet("slow")]
    public async Task<IActionResult> SlowRequest([FromQuery] int delayMs = 500)
    {
        await Task.Delay(delayMs);

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Slow request completed - check logs to see elapsed time",
                delayMs,
                timestamp = DateTime.UtcNow
            },
            "Slow request logging test"
        );

        return Ok(response);
    }
}

public class SensitiveDataRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

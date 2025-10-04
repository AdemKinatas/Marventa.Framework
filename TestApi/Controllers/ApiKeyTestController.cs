using Marventa.Framework.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeyTestController : ControllerBase
{
    private readonly ILogger<ApiKeyTestController> _logger;

    public ApiKeyTestController(ILogger<ApiKeyTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicEndpoint()
    {
        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "This is a public endpoint - no API key required",
                timestamp = DateTime.UtcNow
            },
            "Public access granted"
        );

        return Ok(response);
    }

    /// <summary>
    /// Protected endpoint - requires valid API key
    /// Send header: X-API-Key: test-api-key-12345
    /// </summary>
    [HttpGet("protected")]
    public IActionResult ProtectedEndpoint()
    {
        var identity = User.Identity as ClaimsIdentity;
        var owner = identity?.FindFirst(ClaimTypes.Name)?.Value;
        var roles = identity?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
        var apiKey = identity?.FindFirst("ApiKey")?.Value;

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "API Key authentication successful!",
                owner,
                roles,
                apiKeyUsed = apiKey != null ? $"{apiKey.Substring(0, 4)}****" : null,
                timestamp = DateTime.UtcNow
            },
            "Access granted with API key"
        );

        return Ok(response);
    }

    /// <summary>
    /// Admin-only endpoint - requires API key with Admin role
    /// Send header: X-API-Key: test-api-key-12345 (has Admin role)
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminEndpoint()
    {
        var identity = User.Identity as ClaimsIdentity;
        var owner = identity?.FindFirst(ClaimTypes.Name)?.Value;

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Admin access granted!",
                owner,
                roles = identity?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                timestamp = DateTime.UtcNow
            },
            "Admin-level access successful"
        );

        return Ok(response);
    }

    /// <summary>
    /// User-only endpoint - requires API key with User role
    /// Both test-api-key-12345 and readonly-key-67890 have User role
    /// </summary>
    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public IActionResult UserEndpoint()
    {
        var identity = User.Identity as ClaimsIdentity;
        var owner = identity?.FindFirst(ClaimTypes.Name)?.Value;

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "User access granted!",
                owner,
                roles = identity?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                timestamp = DateTime.UtcNow
            },
            "User-level access successful"
        );

        return Ok(response);
    }

    /// <summary>
    /// Get current API key information
    /// </summary>
    [HttpGet("me")]
    public IActionResult GetCurrentApiKeyInfo()
    {
        var identity = User.Identity as ClaimsIdentity;
        var claimsList = identity?.Claims.Select(c => new { c.Type, c.Value }).ToArray() ?? Array.Empty<object>();

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                authenticationType = User.Identity?.AuthenticationType,
                name = User.Identity?.Name,
                claims = claimsList
            },
            "Current authentication information"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test endpoint to verify API key validation fails properly
    /// Call without X-API-Key header or with invalid key
    /// </summary>
    [HttpGet("validate")]
    public IActionResult ValidateApiKey()
    {
        // If we reach here, API key is valid (middleware authenticated successfully)
        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "API Key is valid",
                owner = User.Identity?.Name,
                timestamp = DateTime.UtcNow
            },
            "API key validated successfully"
        );

        return Ok(response);
    }
}

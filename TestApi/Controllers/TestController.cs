using Marventa.Framework.ApiResponse;
using Marventa.Framework.Core.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("fast")]
    public IActionResult GetFast()
    {
        var responseTime = HttpContext.Items[MiddlewareConstants.ResponseTimeMsProperty] as long? ?? 0;

        var response = ApiResponse<object>.SuccessResponse(
            new { message = "Fast response", timestamp = DateTime.UtcNow },
            "Success"
        );

        response.ResponseTimeMs = responseTime;

        return Ok(response);
    }

    [HttpGet("slow")]
    public async Task<IActionResult> GetSlow()
    {
        await Task.Delay(150); // Simulate slow operation

        var responseTime = HttpContext.Items[MiddlewareConstants.ResponseTimeMsProperty] as long? ?? 0;

        var response = ApiResponse<object>.SuccessResponse(
            new { message = "Slow response (>100ms)", timestamp = DateTime.UtcNow },
            "Success"
        );

        response.ResponseTimeMs = responseTime;

        return Ok(response);
    }

    [HttpGet("cors")]
    public IActionResult GetCors()
    {
        var responseTime = HttpContext.Items[MiddlewareConstants.ResponseTimeMsProperty] as long? ?? 0;

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                message = "CORS test successful",
                origin = HttpContext.Request.Headers["Origin"].ToString(),
                method = HttpContext.Request.Method
            },
            "Success"
        );

        response.ResponseTimeMs = responseTime;

        return Ok(response);
    }

    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new Exception("Test exception");
    }
}

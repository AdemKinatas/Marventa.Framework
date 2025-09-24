using Marventa.Framework.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Marventa.Framework.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Ok<T>(T data, string? message = null)
    {
        return base.Ok(ApiResponse<T>.SuccessResult(data, message));
    }

    protected ActionResult<ApiResponse> Ok(string? message = null)
    {
        return base.Ok(ApiResponse.SuccessResult(null, message));
    }

    protected ActionResult<ApiResponse<T>> BadRequest<T>(string message, string? errorCode = null)
    {
        return base.BadRequest(ApiResponse<T>.FailureResult(message, errorCode));
    }

    protected ActionResult<ApiResponse> BadRequest(string message, string? errorCode = null)
    {
        return base.BadRequest(ApiResponse.FailureResult(message, errorCode));
    }

    protected ActionResult<ApiResponse<T>> NotFound<T>(string message = "Resource not found")
    {
        return base.NotFound(ApiResponse<T>.FailureResult(message, "NOT_FOUND"));
    }

    protected ActionResult<ApiResponse> NotFound(string message = "Resource not found")
    {
        return base.NotFound(ApiResponse.FailureResult(message, "NOT_FOUND"));
    }
}
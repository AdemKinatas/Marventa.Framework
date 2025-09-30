using Marventa.Framework.Core.Application;

namespace Marventa.Framework.ApiResponse;

public static class ApiResponseFactory
{
    public static ApiResponse<T> FromResult<T>(Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse<T>.SuccessResponse(result.Value!, successMessage ?? "Success");
        }

        return ApiResponse<T>.ErrorResponse(result.ErrorMessage!, null);
    }

    public static ApiResponse FromResult(Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse.SuccessResponse(successMessage ?? "Success");
        }

        return ApiResponse.ErrorResponse(result.ErrorMessage!);
    }
}

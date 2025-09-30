namespace BasicWebApi.DTOs;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Succeeded = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Succeeded = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
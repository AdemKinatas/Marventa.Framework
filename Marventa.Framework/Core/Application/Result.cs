namespace Marventa.Framework.Core.Application;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }
    public string? ErrorCode { get; }

    protected Result(bool isSuccess, string? errorMessage = null, string? errorCode = null)
    {
        if (isSuccess && errorMessage != null)
            throw new InvalidOperationException("Success result cannot have error message");

        if (!isSuccess && errorMessage == null)
            throw new InvalidOperationException("Failure result must have error message");

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true);

    public static Result Failure(string errorMessage, string? errorCode = null)
        => new(false, errorMessage, errorCode);

    public static Result<T> Success<T>(T value) => new(value, true);

    public static Result<T> Failure<T>(string errorMessage, string? errorCode = null)
        => new(default!, false, errorMessage, errorCode);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value, bool isSuccess, string? errorMessage = null, string? errorCode = null)
        : base(isSuccess, errorMessage, errorCode)
    {
        Value = value;
    }
}

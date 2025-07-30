using Common.Errors;

namespace Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public Error? Error { get; }
    public T? Value { get; }

    private Result(bool isSuccess, T? value = default, Error? error = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T? value)
    {
        return new Result<T>(isSuccess: true, value: value);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(isSuccess: false, value: default, error: error);
    }
}
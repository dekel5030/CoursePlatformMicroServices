using Common.Errors;

namespace Common;

public readonly record struct Result<T>
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

    public static Result<T> Success(T? value) => new(true, value);
    public static Result<T> Failure(Error error) => new(false, default, error);
}
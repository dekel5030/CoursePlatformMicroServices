using System.Diagnostics.CodeAnalysis;

namespace SharedKernel;

public class Result
{
    [MemberNotNullWhen(false, nameof(Error)), MemberNotNullWhen(true, nameof(Error))]
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    private protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null ||
            !isSuccess && error is null)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result<TValue> Success<TValue>(TValue value) =>
        Result<TValue>.Success(value);

    public static Result Failure(Error error) =>
        new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        Result<TValue>.Failure(error);
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [MemberNotNullWhen(true, nameof(_value))]
    public new bool IsSuccess => base.IsSuccess;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static Result<TValue> Success(TValue value) =>
        new(value, true, null);

    public static new Result<TValue> Failure(Error error) =>
        new(default, false, error);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}

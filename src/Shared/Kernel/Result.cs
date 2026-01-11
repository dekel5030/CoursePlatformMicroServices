using System.Diagnostics.CodeAnalysis;

namespace Kernel;

public class Result
{
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Error))]
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
    private Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    public new bool IsSuccess => base.IsSuccess;

    [MemberNotNullWhen(false, nameof(Value)), MemberNotNullWhen(true, nameof(Error))]
    public new bool IsFailure => !base.IsSuccess;

    public new Error? Error => base.Error;

    public TValue? Value => IsSuccess
        ? field
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static Result<TValue> Success(TValue value) =>
        new(value, true, null);

    public static new Result<TValue> Failure(Error error) =>
        new(default, false, error);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}

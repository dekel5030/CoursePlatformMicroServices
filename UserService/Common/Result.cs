namespace UserService.Common
{
    public record Result<T>
    {
        public bool IsSuccess { get; init; }
        public ErrorCode? ErrorCode { get; init; }
        public T? Value { get; init; }

        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
        public static Result<T> Failure(ErrorCode ErrorCode) => new() { IsSuccess = false, ErrorCode = ErrorCode };
    }
}
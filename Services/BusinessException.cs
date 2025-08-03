namespace UserService.Services;

public class BusinessException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public BusinessException(string code, string message, int statusCode = 422)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}
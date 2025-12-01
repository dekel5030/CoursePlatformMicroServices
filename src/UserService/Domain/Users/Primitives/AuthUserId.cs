namespace Domain.Users.Primitives;

public class AuthUserId
{
    public string? Value { get; }

    public AuthUserId(string? value)
    {
        Value = value;
    }
}
namespace Users.Domain.Users.Primitives;

public record struct UserId(Guid Value)
{
    public override string ToString() => Value.ToString();
};

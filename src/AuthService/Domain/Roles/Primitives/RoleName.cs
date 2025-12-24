namespace Auth.Domain.Roles.Primitives;

public record RoleName
{
    public string Value { get; }

    public RoleName(string value)
    {
        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;
}

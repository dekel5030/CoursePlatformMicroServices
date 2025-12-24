namespace Auth.Domain.Roles.Primitives;

public record RoleName
{
    public string Value { get; }

    public RoleName(string value)
    {
        Value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator string(RoleName roleName) => roleName.Value;
    public override string ToString() => Value;
}

namespace Kernel.Auth.AuthTypes;

public record ResourceId
{
    public string Value { get; }

    public static readonly ResourceId Wildcard = new("*");

    private ResourceId(string value)
    {
        Value = value;
    }

    public static ResourceId Create(string value)
    {
        if (value == "*")
        {
            return Wildcard;
        }

        return new ResourceId(value);
    }

    public bool IsWildcard => Value == "*";

    public override string ToString() => Value;
}

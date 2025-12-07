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
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Resource ID cannot be empty.");

        if (value == "*")
            return Wildcard;

        if (value.Contains(':'))
            throw new ArgumentException("Resource ID cannot contain the delimiter ':'.");

        return new ResourceId(value.ToLowerInvariant());
    }

    public bool IsWildcard => Value == "*";

    public override string ToString() => Value;
}
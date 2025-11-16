namespace Domain.Users.Primitives;
public record PhoneNumber(string CountryCode, string Number)
{
    public override string ToString() => $"{CountryCode} {Number}";
}
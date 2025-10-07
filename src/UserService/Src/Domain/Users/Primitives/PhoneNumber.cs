namespace Domain.Users.Primitives;
public record struct PhoneNumber(string CountryCode, string Number)
{
    public override string ToString() => $"{CountryCode} {Number}";
}
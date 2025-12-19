namespace Infrastructure.Auth.Jwt;

internal class InternalTokenOptions
{
    public const string SectionName = "InternalToken";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

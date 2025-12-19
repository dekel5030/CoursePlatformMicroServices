namespace Gateway.Api.Jwt;

public class KeycloakJwtOptions
{
    public const string SectionName = "Jwt";

    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
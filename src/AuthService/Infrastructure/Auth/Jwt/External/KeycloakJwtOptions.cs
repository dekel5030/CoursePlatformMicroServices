namespace Infrastructure.Auth.Jwt.External;

public class KeycloakJwtOptions
{
    public const string SectionName = "Keycloak";

    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
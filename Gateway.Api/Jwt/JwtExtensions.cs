namespace Gateway.Api.Jwt;

public static class JwtExtensions
{
    public static IServiceCollection ConfigureKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KeycloakJwtOptions>(configuration.GetSection(KeycloakJwtOptions.SectionName));
        services.ConfigureOptions<KeyCloakOptionsSetup>();

        return services;
    }
}
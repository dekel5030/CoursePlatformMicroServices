namespace Gateway.Api.Jwt;

public static class JwtExtensions
{
    public static IServiceCollection ConfigureJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        return services;
    }
}
using Application.Abstractions.Auth;
using Infrastructure.Auth.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auth;

internal static class HostApplicationExtensions
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<KeyManager>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IPermissionResolver, PermissionResolver>();

        return services;
    }
}

using Common.Auth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Auth.Extentions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureOptions<JwtOptionsSetup>();
        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidation>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        return services;
    }
}

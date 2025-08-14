using Common.Web.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        services.AddOptions<SwaggerOptions>()
            .BindConfiguration(SwaggerOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.ConfigureOptions<SwaggerOptionsSetup>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.ConfigureOptions<SwaggerGenOptionsSetup>();

        return services;
    }
}

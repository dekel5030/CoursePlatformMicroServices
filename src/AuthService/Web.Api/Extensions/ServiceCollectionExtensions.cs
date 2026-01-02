using Auth.Infrastructure.Auth.Jwt;
using CoursePlatform.ServiceDefaults.Swagger;

namespace Auth.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static TBuilder AddSwaggerGenWithAuth<TBuilder>(this TBuilder builder) 
        where TBuilder : IHostApplicationBuilder
    {
        SwaggerExtensions.AddDefaultOpenApi<TBuilder>(builder, AuthSchemes.Internal);

        return builder;
    }
}
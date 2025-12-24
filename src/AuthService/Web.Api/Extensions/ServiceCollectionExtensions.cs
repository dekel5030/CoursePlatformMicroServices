using Auth.Infrastructure.Auth.Jwt;
using CoursePlatform.ServiceDefaults;

namespace Auth.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static TBuilder AddSwaggerGenWithAuth<TBuilder>(this TBuilder builder) 
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddDefaultOpenApi(AuthSchemes.Internal);

        return builder;
    }
}
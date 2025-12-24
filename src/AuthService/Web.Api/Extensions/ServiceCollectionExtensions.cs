using Auth.Infrastructure.Auth.Jwt;
using Microsoft.OpenApi.Models;

namespace Auth.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token: Bearer {your_token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            o.AddSecurityDefinition(AuthSchemes.Internal, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement();

            var schemeReference = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = AuthSchemes.Internal
                }
            };

            securityRequirement.Add(schemeReference, Array.Empty<string>());

            o.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}
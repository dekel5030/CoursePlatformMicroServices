using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CoursePlatform.ServiceDefaults.Swagger;

public static class SwaggerExtensions
{
    public static TBuilder AddDefaultOpenApi<TBuilder>(
        this TBuilder builder,
        string securitySchemeId = "Keycloak")
            where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OperationFilter<ProblemDetailsOperationFilter>(securitySchemeId);
            options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var scheme = new OpenApiSecurityScheme
            {
                Name = securitySchemeId,
                Description = $"Enter {securitySchemeId} JWT token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition(securitySchemeId, scheme);
        });

        return builder;
    }
}
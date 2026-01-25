using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoursePlatform.ServiceDefaults.Swagger;

public static class SwaggerExtensions
{
    public static TBuilder AddDefaultOpenApi<TBuilder>(
        this TBuilder builder,
        string securitySchemeId = "Keycloak")
            where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OperationFilter<ProblemDetailsOperationFilter>(securitySchemeId);

            options.SchemaFilter<EnumSchemaFilter>();

            options.CustomSchemaIds(id => id.FullName?.Replace("+", "-", StringComparison.Ordinal));

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

internal sealed class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema concreteSchema)
        {
            return;
        }

        if (context.Type != null && context.Type.IsEnum)
        {
            concreteSchema.Enum?.Clear();

            concreteSchema.Enum ??= new List<JsonNode>();

            concreteSchema.Type = JsonSchemaType.String;
            concreteSchema.Format = null;

            foreach (string name in Enum.GetNames(context.Type))
            {
                concreteSchema.Enum.Add(JsonValue.Create(name));
            }
        }
    }
}

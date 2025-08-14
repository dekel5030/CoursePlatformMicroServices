using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Web.Swagger;

public sealed class SwaggerGenOptionsSetup : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IOptions<SwaggerOptions> _opt;
    public SwaggerGenOptionsSetup(IOptions<SwaggerOptions> opt) => _opt = opt;

    public void Configure(SwaggerGenOptions options)
    {
        var sw = _opt.Value;

        options.SwaggerDoc(sw.Version, new OpenApiInfo
        {
            Title = sw.Title,
            Version = sw.Version,
            Description = sw.Description
        });

        if (sw.Auth.Enabled)
        {
            var inLoc = sw.Auth.In?.ToLowerInvariant() switch
            {
                "query"  => ParameterLocation.Query,
                "cookie" => ParameterLocation.Cookie,
                _        => ParameterLocation.Header
            };

            options.AddSecurityDefinition(sw.Auth.SchemeName, new OpenApiSecurityScheme
            {
                Name = sw.Auth.HeaderName,
                Type = SecuritySchemeType.Http,
                Scheme = sw.Auth.Scheme,
                BearerFormat = sw.Auth.BearerFormat,
                In = inLoc,
                Description = sw.Auth.Description
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme, Id = sw.Auth.SchemeName
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        options.CustomSchemaIds(t => t.FullName);
    }
}
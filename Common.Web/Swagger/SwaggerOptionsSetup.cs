using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Common.Web.Swagger;

public class SwaggerOptionsSetup : IConfigureOptions<SwaggerOptions>
{
    private readonly IConfiguration _configuration;
    public SwaggerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(SwaggerOptions options)
    {
        _configuration.GetSection(SwaggerOptions.SectionName).Bind(options);
    }
}
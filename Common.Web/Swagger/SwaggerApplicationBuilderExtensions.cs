using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Web.Swagger;

public static class SwaggerApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder app)
    {
        var sw = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;

        app.UseSwagger();

        if (sw.EnableUI)
        {
            app.UseSwaggerUI(ui =>
            {
                ui.DocumentTitle = sw.Title;
                ui.SwaggerEndpoint($"/swagger/{sw.Version}/swagger.json", $"{sw.Title} {sw.Version}");
                ui.RoutePrefix = sw.RoutePrefix;
            });
        }

        return app;
    }
}

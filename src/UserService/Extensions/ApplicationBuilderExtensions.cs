using Common.Web.Swagger;
using UserService.SyncDataServices.Grpc;

namespace UserService.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseUserServiceDependencies(this IApplicationBuilder app)
    {
        app.UseLocalization();

        app.UseAppSwagger();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapUserServiceEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapGrpcService<GrpcUserService>();

        return app;
    }

    private static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
    {
        var supportedCultures = new[] { "en", "he" };

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture("he")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        return app;
    }
    
}
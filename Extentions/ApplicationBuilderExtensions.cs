namespace AuthService.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseLocalizationSetup(this IApplicationBuilder app)
    {
        var supportedCultures = new[] { "en", "he" };

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture("en")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}

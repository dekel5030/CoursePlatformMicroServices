namespace Courses.Api.Infrastructure.LinkProvider;

internal static class LinkServiceExtensions
{
    public static IServiceCollection AddLinkProvider(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<LinkProvider>();
        return services;
    }
}

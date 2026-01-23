using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Infrastructure.LinkProvider;

internal static class LinkServiceExtensions
{
    public static IServiceCollection AddLinkProvider(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.Scan(scan => scan
            .FromAssemblyOf<ModuleLinkProvider>()
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IResourceLinkProvider<>)),
                publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());


        return services;
    }
}

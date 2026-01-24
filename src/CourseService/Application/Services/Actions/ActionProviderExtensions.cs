using Courses.Application.Services.Actions.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Services.Actions;

internal static class ActionProviderExtensions
{
    public static IServiceCollection AddActionProvider(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(AssemblyMarker))
            .AddClasses(classes => classes.AssignableTo(typeof(IActionRule<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IActionProvider<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}

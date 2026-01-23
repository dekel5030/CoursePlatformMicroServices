using Courses.Application.Actions.Abstractions;
using Courses.Application.Actions.CourseCollection.Policies;
using Courses.Application.Actions.Courses;
using Courses.Application.Actions.Courses.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Actions;

internal static class ActionProviderExtensions
{
    public static IServiceCollection AddActionProvider(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(Application.AssemblyMarker))
            .AddClasses(classes => classes.AssignableTo(typeof(IActionRule<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IActionProvider<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}

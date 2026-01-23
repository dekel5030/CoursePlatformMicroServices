using Courses.Application.Actions.Abstract;
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
            .FromAssemblyOf<Application.AssemblyMarker>()
            .AddClasses(classes => classes.AssignableTo(typeof(IActionRule<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.Scan(scan => scan
            .FromAssemblyOf<Application.AssemblyMarker>()
            .AddClasses(classes => classes.AssignableTo(typeof(IActionProvider<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}

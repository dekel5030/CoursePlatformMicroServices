using Courses.Application.Actions.Abstract;
using Courses.Application.Actions.Policies.CourseCollection;
using Courses.Application.Actions.Policies.Courses;
using Courses.Application.Actions.Policies.Lessons;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Actions;

internal static class ActionProviderExtensions
{
    public static IServiceCollection AddActionProvider(this IServiceCollection services)
    {
        services.Scan(scan => scan
                .FromAssemblyOf<Application.AssemblyMarker>()
                .AddClasses(classes => classes.AssignableTo<ICourseActionRule>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        services.Scan(scan => scan
                .FromAssemblyOf<Application.AssemblyMarker>()
                .AddClasses(classes => classes.AssignableTo<ICourseCollectionActionRule>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        services.Scan(scan => scan
                .FromAssemblyOf<Application.AssemblyMarker>()
                .AddClasses(classes => classes.AssignableTo<ILessonActionRule>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        services.AddScoped<ICourseActionProvider, CourseActionProvider>();

        return services;
    }
}

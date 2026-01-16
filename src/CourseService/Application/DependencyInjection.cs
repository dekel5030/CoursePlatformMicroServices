using CoursePlatform.ServiceDefaults.Messaging;
using Courses.Application.Actions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddMediator<AssemblyMarker>();

        services.AddActionProvider();

        return services;
    }
}

public class AssemblyMarker
{
}

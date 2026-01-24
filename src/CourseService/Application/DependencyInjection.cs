using CoursePlatform.ServiceDefaults.Messaging;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
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

        services.AddLinkFactories();

        return services;
    }
}

public class AssemblyMarker
{
}

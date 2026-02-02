using CoursePlatform.ServiceDefaults.Messaging;
using CoursePlatform.ServiceDefaults.Messaging.Behaviors;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using FluentValidation;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddMediator<AssemblyMarker>();
        services.AddScoped<LessonManagementService>();
        services.AddScoped<ModuleManagementService>();

        services.AddOpenBehavior(typeof(LoggingBehavior<,>));
        services.AddActionProvider();

        services.AddLinkBuilder();

        return services;
    }
}

public class AssemblyMarker
{
}

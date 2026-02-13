using CoursePlatform.ServiceDefaults.Messaging;
using CoursePlatform.ServiceDefaults.Messaging.Behaviors;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.ReadModels;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddMediator<AssemblyMarker>();
        services.AddScoped<ICourseAnalyticsProjection, CourseAnalyticsProjectionService>();
        services.AddScoped<LessonManagementService>();
        services.AddScoped<ModuleManagementService>();

        services.AddOpenBehavior(typeof(LoggingBehavior<,>));
        services.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
        services.AddActionProvider();

        services.AddLinkBuilder();

        services.AddScoped<ICoursePageDtoMapper, CoursePageDtoMapper>();
        services.AddScoped<ILessonPageDtoMapper, LessonPageDtoMapper>();
        services.AddScoped<ICourseSummaryDtoMapper, CourseSummaryDtoMapper>();
        services.AddScoped<ICoursePageDataLoader, CoursePageDataLoader>();

        return services;
    }
}

public class AssemblyMarker
{
}

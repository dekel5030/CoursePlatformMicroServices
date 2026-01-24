using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Services.LinkProvider;

internal static class LinkExtensions
{
    public static IServiceCollection AddLinkFactories(this IServiceCollection services)
    {
        services.AddScoped<ICourseLinkFactory, CourseLinkFactory>();
        services.AddScoped<IModuleLinkFactory, ModuleLinkFactory>();
        services.AddScoped<ILessonLinkFactory, LessonLinkFactory>();

        return services;
    }
}

using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Services.LinkProvider;

internal static class LinkExtensions
{
    public static IServiceCollection AddLinkBuilder(this IServiceCollection services)
    {
        services.AddScoped<ILinkDefinitionRegistry, CourseLinkDefinitions>();
        services.AddScoped<ILinkDefinitionRegistry, ModuleLinkDefinitions>();
        services.AddScoped<ILinkDefinitionRegistry, LessonLinkDefinitions>();
        services.AddScoped<ILinkDefinitionRegistry, CourseCollectionLinkDefinitions>();
        services.AddScoped<ILinkDefinitionRegistry, CourseRatingEligibilityLinkDefinitions>();
        services.AddScoped<ILinkDefinitionRegistry, CourseRatingLinkDefinitions>();
        services.AddScoped<ILinkBuilderService, LinkBuilderService>();

        return services;
    }
}

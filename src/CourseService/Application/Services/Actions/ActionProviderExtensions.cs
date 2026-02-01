using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Services.Actions;

internal static class ActionProviderExtensions
{
    public static IServiceCollection AddActionProvider(this IServiceCollection services)
    {
        services.AddScoped<CourseGovernancePolicy>();

        return services;
    }
}

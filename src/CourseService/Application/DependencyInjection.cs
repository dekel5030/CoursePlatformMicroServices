using CoursePlatform.ServiceDefaults.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddMediator<AssemblyMarker>();

        return services;
    }
}


public class AssemblyMarker
{
}
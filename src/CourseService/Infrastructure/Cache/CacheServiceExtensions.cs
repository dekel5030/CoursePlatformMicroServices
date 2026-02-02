using CoursePlatform.ServiceDefaults.Messaging.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure.Cache;

internal static class CacheServiceExtensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}

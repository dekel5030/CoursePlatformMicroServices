using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth.Infrastructure.Redis.Extensions;

internal static class HostApplicationExtensions
{
    public static IHostApplicationBuilder AddDistributedPermissionStore(
        this IHostApplicationBuilder builder,
        string redisConnectionString)
    {
        builder.AddRedisDistributedCache(redisConnectionString);

        builder.Services.AddSingleton<Application.Abstractions.Caching.ICacheService, RedisCache>();

        return builder;
    }
}

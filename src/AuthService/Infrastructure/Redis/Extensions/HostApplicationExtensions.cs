using Application.Abstractions.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Redis.Extensions;

internal static class HostApplicationExtensions
{
    public static IHostApplicationBuilder AddDistributedPermissionStore(
        this IHostApplicationBuilder builder, 
        string redisConnectionString)
    {
        builder.AddRedisClient(redisConnectionString);

        builder.Services.AddStackExchangeRedisCache(options => { });

        builder.Services.AddSingleton<ICacheService, RedisCache>();

        return builder;
    }
}

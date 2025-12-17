using Microsoft.Extensions.Hosting;

namespace Infrastructure.Redis.Extensions;

internal static class HostApplicationExtensions
{
    public static IHostApplicationBuilder AddDistributedPermissionStore(
        this IHostApplicationBuilder builder, 
        string redisConnectionString)
    {
        builder.AddRedisClient(redisConnectionString);

        return builder;
    }
}

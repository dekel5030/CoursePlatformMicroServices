using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Infrastructure.Redis.EventHandlers;
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

        builder.Services.AddScoped<IDomainEventHandler<RoleCreatedDomainEvent>, RoleCacheInvalidationHandler>();
        builder.Services.AddScoped<IDomainEventHandler<RolePermissionAddedDomainEvent>, RoleCacheInvalidationHandler>();
        builder.Services.AddScoped<IDomainEventHandler<RolePermissionRemovedDomainEvent>, RoleCacheInvalidationHandler>();

        builder.Services.AddScoped<IRolePermissionsCacheWriter, RedisRolePermissionsCacheWriter>();

        return builder;
    }
}

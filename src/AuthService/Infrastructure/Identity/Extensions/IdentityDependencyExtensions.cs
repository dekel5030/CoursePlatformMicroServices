using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Domain.Roles.Events;
using Infrastructure.Identity.SyncEventsHandlers.Roles;
using Infrastructure.Identity.SyncEventsHandlers.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Extensions;

internal static class IdentityDependencyExtensions
{
    public static IServiceCollection AddIdentitySyncHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<RoleCreatedDomainEvent>, RoleCreatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<RolePermissionRemovedDomainEvent>, RolePermissionRemovedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<RolePermissionAssignedDomainEvent>, RolePermissionAssignedDomainEventHandler>();
        
        services.AddScoped<IDomainEventHandler<UserRoleAddedDomainEvent>, UserRoleAddedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<UserRoleRemovedDomainEvent>, UserRoleRemovedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<UserPermissionAddedDomainEvent>, UserPermissionAddedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<UserPermissionRemovedDomainEvent>, UserPermissionRemovedDomainEventHandler>();
        return services;
    }
}

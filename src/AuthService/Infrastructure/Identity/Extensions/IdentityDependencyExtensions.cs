using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Infrastructure.Identity.SyncEvents;
using Infrastructure.Identity.SyncEventsHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Extensions;

internal static class IdentityDependencyExtensions
{
    public static IServiceCollection AddIdentitySyncHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<RoleCreatedDomainEvent>, RoleCreatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<RolePermissionRemovedDomainEvent>, RolePermissionRemovedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<RolePermissionAssignedDomainEvent>, RolePermissionAssignedDomainEventHandler>();
    
        return services;
    }
}

using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Infrastructure.Redis.EventCollector;

namespace Infrastructure.Redis.EventHandlers;

internal class RoleCacheInvalidationHandler :
    IDomainEventHandler<RoleCreatedDomainEvent>,
    IDomainEventHandler<RolePermissionAddedDomainEvent>,
    IDomainEventHandler<RolePermissionRemovedDomainEvent>
{
    private readonly IRoleEventsCollector _collector;

    public RoleCacheInvalidationHandler(IRoleEventsCollector roleEventsCollector)
    {
        _collector = roleEventsCollector;
    }

    public Task Handle(
        RolePermissionRemovedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        return HandleRoleChangedAsync(@event.Role.Name, cancellationToken);
    }

    public Task Handle(
        RolePermissionAddedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        return HandleRoleChangedAsync(@event.Role.Name, cancellationToken);
    }

    public Task Handle(
        RoleCreatedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        return HandleRoleChangedAsync(@event.Role.Name, cancellationToken);
    }

    private Task HandleRoleChangedAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        _collector.MarkRoleForRefresh(roleName);
        return Task.CompletedTask;
    }
}

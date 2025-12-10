using Application.Abstractions.Messaging;
using Auth.Contracts.Redis.Events;
using Domain.Roles.Events;

namespace Infrastructure.Redis.EventHandlers;

internal class RoleCacheInvalidationHandler :
    IDomainEventHandler<RoleCreatedDomainEvent>,
    IDomainEventHandler<RolePermissionAddedDomainEvent>,
    IDomainEventHandler<RolePermissionRemovedDomainEvent>
{
    private readonly IEventPublisher _publisher;

    public RoleCacheInvalidationHandler(IEventPublisher publisher)
    {
        _publisher = publisher;
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
        return _publisher.PublishAsync(
            new RolePermissionsChangedEvent(roleName),
            cancellationToken);
    }
}

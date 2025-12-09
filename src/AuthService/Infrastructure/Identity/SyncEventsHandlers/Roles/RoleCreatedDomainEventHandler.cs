using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.SyncEventsHandlers.Roles;

internal class RoleCreatedDomainEventHandler : IDomainEventHandler<RoleCreatedDomainEvent>
{
    private readonly RoleManager<ApplicationIdentityRole> _roleManager;
    
    public RoleCreatedDomainEventHandler(RoleManager<ApplicationIdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public Task Handle(
        RoleCreatedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        var identityRole = new ApplicationIdentityRole(@event.Role);

        return _roleManager.CreateAsync(identityRole);
    }
}

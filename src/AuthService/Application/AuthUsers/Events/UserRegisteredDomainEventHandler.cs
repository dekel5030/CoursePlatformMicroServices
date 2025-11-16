using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Auth.Contracts.Events;
using Domain.AuthUsers.Events;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Events;

public class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IEventPublisher _eventPublisher;

    public UserRegisteredDomainEventHandler(
        IReadDbContext readDbContext,
        IEventPublisher eventPublisher)
    {
        _readDbContext = readDbContext;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(UserRegisteredDomainEvent request, CancellationToken cancellationToken = default)
    {
        // Load the auth user with roles and permissions
        var authUser = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == request.AuthUserId, cancellationToken);

        if (authUser == null)
        {
            throw new InvalidOperationException($"AuthUser with Id {request.AuthUserId.Value} not found");
        }

        // Extract roles and permissions
        var roles = authUser.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var permissionsFromRoles = authUser.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name);

        var directPermissions = authUser.UserPermissions
            .Select(up => up.Permission.Name);

        var allPermissions = permissionsFromRoles
            .Concat(directPermissions)
            .Distinct()
            .ToList();

        // Publish integration event to UserService
        var integrationEvent = new UserRegisteredV1(
            authUser.Id.Value.ToString(),
            authUser.Email,
            request.RegisteredAt,
            roles,
            allPermissions);

        await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}

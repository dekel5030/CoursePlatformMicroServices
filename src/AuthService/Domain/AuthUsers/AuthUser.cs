using Domain.AuthUsers.Events;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Domain.AuthUsers;

public class AuthUser : IdentityUser<Guid>, IHasDomainEvents
{
    private readonly DomainEventContainer _eventHandler = new();

    private AuthUser() { }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _eventHandler.DomainEvents;
    public void ClearDomainEvents() => _eventHandler.ClearDomainEvents();
    public void Raise(IDomainEvent domainEvent) => _eventHandler.Raise(domainEvent);

    public static AuthUser Create(string email, string? userName)
    {
        var authUser = new AuthUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = string.IsNullOrEmpty(userName) ? email : userName
        };

        authUser.Raise(new UserRegisteredDomainEvent(
            authUser.Id,
            email,
            DateTime.UtcNow));

        return authUser;
    }
}

public class DomainEventContainer : Entity { }

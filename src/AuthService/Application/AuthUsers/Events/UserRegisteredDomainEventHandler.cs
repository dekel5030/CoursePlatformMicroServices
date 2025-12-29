using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Events;
using CoursePlatform.Contracts.AuthEvents;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Events;

internal class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly IEventBus _eventPublisher;

    public UserRegisteredDomainEventHandler(IEventBus eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public Task HandleAsync(
        UserRegisteredDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        AuthUser user = @event.User;
        var contract = new UserRegisteredEvent(
            user.Id.Value, 
            user.FullName.FirstName.Name, 
            user.FullName.LastName.Name, 
            user.Email.Address);

        return _eventPublisher.PublishAsync(contract, cancellationToken);
    }
}

using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Events;
using CoursePlatform.Contracts.AuthEvents;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Events;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly IEventBus _eventPublisher;

    public UserRegisteredDomainEventHandler(IEventBus eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public Task HandleAsync(
        UserRegisteredDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        AuthUser user = message.User;
        var contract = new UserRegisteredEvent(
            user.Id.Value,
            user.FullName.FirstName.Name,
            user.FullName.LastName.Name,
            user.Email.Address);

        return _eventPublisher.PublishAsync(contract, cancellationToken);
    }
}

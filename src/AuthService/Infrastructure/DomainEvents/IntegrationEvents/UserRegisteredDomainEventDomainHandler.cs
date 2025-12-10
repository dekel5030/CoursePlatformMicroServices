using Application.Abstractions.Messaging;
using Auth.Contracts.Events;
using Domain.AuthUsers.Events;

namespace Infrastructure.DomainEvents.IntegrationEvents;

public class UserRegisteredDomainEventDomainHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly IEventPublisher _eventPublisher;

    public UserRegisteredDomainEventDomainHandler(
        IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(
        UserRegisteredDomainEvent request, 
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new UserRegistered(
            request.User.Id.ToString(),
            request.User.Id.ToString(),
            request.User.Email,
            DateTime.UtcNow);

        await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}

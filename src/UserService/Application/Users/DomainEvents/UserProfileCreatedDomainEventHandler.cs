using Application.Abstractions.Messaging;
using Domain.Users.Events;
using Users.Contracts.Events;

namespace Application.Users.DomainEvents;

public class UserProfileCreatedDomainEventHandler(
    IEventPublisher eventPublisher) : IDomainEventHandler<UserProfileCreatedDomainEvent>
{
    public async Task Handle(
        UserProfileCreatedDomainEvent request, 
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new UserCreated(
            request.UserId.Value.ToString(),
            string.Empty, // Fullname can be added later when available
            request.Email,
            request.CreatedAt);

        await eventPublisher.Publish(integrationEvent, cancellationToken);
    }
}

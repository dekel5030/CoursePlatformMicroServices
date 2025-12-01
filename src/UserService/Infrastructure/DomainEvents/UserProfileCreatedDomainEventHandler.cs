using Application.Abstractions.Messaging;
using Domain.Users.Events;
using MassTransit;
using Users.Contracts.Events;

namespace Infrastructure.DomainEvents;

public class UserProfileCreatedDomainEventHandler(
    IPublishEndpoint publishEndpoint) : IDomainEventHandler<UserProfileCreatedDomainEvent>
{
    public async Task Handle(
        UserProfileCreatedDomainEvent request,
        CancellationToken cancellationToken = default)
    {
        // Map domain event to integration event contract
        var integrationEvent = new UserCreated(
            request.UserId.Value.ToString(),
            string.Empty, // Fullname can be added later when available
            request.Email,
            request.CreatedAt);

        // Publish the contract to the message bus
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}
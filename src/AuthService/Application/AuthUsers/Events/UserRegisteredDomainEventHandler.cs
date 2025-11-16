using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Auth.Contracts.Events;
using Domain.AuthUsers.Events;

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
        var integrationEvent = new UserRegistered(
            request.AuthUserId.Value.ToString(),
            request.Email,
            request.RegisteredAt);

        await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}

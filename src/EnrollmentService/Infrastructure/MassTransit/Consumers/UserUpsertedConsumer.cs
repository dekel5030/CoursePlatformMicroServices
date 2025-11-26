using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents;
using MassTransit;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

public class UserUpsertedConsumer : IConsumer<UserUpserted>
{
    private readonly IIntegrationEventHandler<UserUpsertedIntegrationEvent> _handler;

    public UserUpsertedConsumer(
        IIntegrationEventHandler<UserUpsertedIntegrationEvent> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<UserUpserted> context)
    {
        var integrationEvent = new UserUpsertedIntegrationEvent(
            context.Message.UserId,
            context.Message.Fullname,
            context.Message.IsActive);

        await _handler.Handle(integrationEvent, context.CancellationToken);
    }
}
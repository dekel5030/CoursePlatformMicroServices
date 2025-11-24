using Application.Users.IntegrationEvents;
using MassTransit;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

public class UserUpsertedConsumer : IConsumer<UserUpsertedV1>
{
    private readonly Application.Abstractions.Messaging.IIntegrationEventHandler<UserUpsertedIntegrationEvent> _handler;

    public UserUpsertedConsumer(
        Application.Abstractions.Messaging.IIntegrationEventHandler<UserUpsertedIntegrationEvent> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<UserUpsertedV1> context)
    {
        var integrationEvent = new UserUpsertedIntegrationEvent(
            context.Message.UserId,
            context.Message.Fullname,
            context.Message.IsActive);

        await _handler.Handle(integrationEvent, context.CancellationToken);
    }
}

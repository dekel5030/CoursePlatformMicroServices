using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents.UserUpserted;
using Infrastructure.Database;
using Kernel;
using MassTransit;
using Messaging.EventEnvelope;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

internal sealed class UserUpsertedConsumer : IConsumer<EventEnvelope<UserUpsertedV1>>
{
    private readonly IIntegrationEventHandler<UserUpsertedIntegrationEvent> _handler;

    public UserUpsertedConsumer(IIntegrationEventHandler<UserUpsertedIntegrationEvent> handler)
    {
        _handler = handler;
    }

    public Task Consume(ConsumeContext<EventEnvelope<UserUpsertedV1>> context)
    {
        EventEnvelope<UserUpsertedV1> message = context.Message;

        var @event = new UserUpsertedIntegrationEvent(
            UserId: message.Payload.UserId,
            Email: message.Payload.Email,
            Fullname: message.Payload.Fullname,
            IsActive: message.Payload.IsActive,
            AggregateVersion: message.AggregateVersion,
            OccurredAt: message.OccurredAtUtc
        );

        _handler.Handle(@event);

        return Task.CompletedTask;
    }
}


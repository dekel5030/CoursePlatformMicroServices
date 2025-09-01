using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents.UserUpserted;
using MassTransit;
using Messaging.EventEnvelope;
using Microsoft.Extensions.Logging;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

internal sealed class UserUpsertedConsumer(
    IIntegrationEventHandler<UserUpsertedIntegrationEvent> handler,
    ILogger<UserUpsertedConsumer> logger) 
        : IConsumer<EventEnvelope<UserUpsertedV1>>
{
    public Task Consume(ConsumeContext<EventEnvelope<UserUpsertedV1>> context)
    {
        logger.LogInformation("Received UserUpsertedV1 event for UserId: {UserId}", context.Message.Payload.UserId);

        EventEnvelope<UserUpsertedV1> message = context.Message;

        var @event = new UserUpsertedIntegrationEvent(
            UserId: message.Payload.UserId,
            Email: message.Payload.Email,
            Fullname: message.Payload.Fullname,
            IsActive: message.Payload.IsActive,
            AggregateVersion: message.AggregateVersion,
            OccurredAt: message.OccurredAtUtc
        );

        return handler.Handle(@event, context.CancellationToken);
    }
}
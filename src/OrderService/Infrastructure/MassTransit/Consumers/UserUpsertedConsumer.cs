using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents.UserUpserted;
using MassTransit;
using Microsoft.Extensions.Logging;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

internal sealed class UserUpsertedConsumer(
    IIntegrationEventHandler<UserUpsertedIntegrationEvent> handler,
    ILogger<UserUpsertedConsumer> logger) 
        : IConsumer<UserUpsertedV1>
{
    public Task Consume(ConsumeContext<UserUpsertedV1> context)
    {
        logger.LogInformation("Received UserUpsertedV1 event for UserId: {UserId}", context.Message.UserId);

        UserUpsertedV1 message = context.Message;

        var @event = new UserUpsertedIntegrationEvent(
            UserId: message.UserId,
            Email: message.Email,
            Fullname: message.Fullname,
            IsActive: message.IsActive,
            AggregateVersion: 1 //message.AggregateVersion
        );

        return handler.Handle(@event, context.CancellationToken);
    }
}
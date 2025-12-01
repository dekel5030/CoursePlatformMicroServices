using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents.AuthRegistered;
using Auth.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MassTransit.Consumers;

public class AuthRegisteredConsumer(
    IIntegrationEventHandler<AuthRegisteredIntegrationEvent> eventHandler,
    ILogger<AuthRegisteredConsumer> logger) : IConsumer<UserRegistered>
{
    public Task Consume(ConsumeContext<UserRegistered> context)
    {
        logger.LogInformation(
            "Received UserRegistered event for AuthUserId: {AuthUserId}, UserId: {UserId}, Email: {Email}",
            context.Message.AuthUserId,
            context.Message.UserId,
            context.Message.Email);

        UserRegistered message = context.Message;

        AuthRegisteredIntegrationEvent @event = new(
            message.AuthUserId,
            message.UserId,
            message.Email,
            null); // Username is not in UserRegistered event

        return eventHandler.Handle(@event);
    }
}
using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents.AuthRegistered;
using Auth.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MassTransit.Consumers;

public class AuthRegisteredConsumer(
    IIntegrationEventHandler<AuthRegisteredIntegrationEvent> eventHandler,
    ILogger<AuthRegisteredConsumer> logger) : IConsumer<AuthRegistered>
{
    public Task Consume(ConsumeContext<AuthRegistered> context)
    {
        logger.LogInformation(
            "Received AuthRegistered event for Email: {Email}, Username: {Username}",
            context.Message.Email,
            context.Message.Username ?? "N/A");

        AuthRegistered message = context.Message;

        AuthRegisteredIntegrationEvent @event = new(
            message.Email,
            message.Username);

        return eventHandler.Handle(@event);
    }
}

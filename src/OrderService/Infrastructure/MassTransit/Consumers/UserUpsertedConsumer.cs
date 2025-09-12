using Application.Abstractions.Messaging;
using Application.Users.IntegrationEvents.UserUpserted;
using Domain.Users;
using Domain.Users.Primitives;
using Infrastructure.Database;
using MassTransit;
using Microsoft.Extensions.Logging;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

public sealed class UserUpsertedConsumer(
    IIntegrationEventHandler<UserUpsertedIntegrationEvent> handler,
    ReadDbContext dbContext,
    ILogger<UserUpsertedConsumer> logger) 
        : IConsumer<UserUpsertedV1>
{
    public Task Consume(ConsumeContext<UserUpsertedV1> context)
    {
        logger.LogInformation("Received UserUpsertedV1 event for UserId: {UserId}", context.Message.UserId);

        UserUpsertedV1 message = context.Message;

        User? user = dbContext.Users
            .SingleOrDefault(u => u.ExternalUserId == new ExternalUserId(message.UserId));

        long currentVersion = user?.EntityVersion ?? 0;
        long messageVersion = message.EntityVersion;

        var @event = new UserUpsertedIntegrationEvent(
                UserId: message.UserId,
                Email: message.Email,
                Fullname: message.Fullname,
                IsActive: message.IsActive,
                EntityVersion: message.EntityVersion
            );

        return VersionChecker.CheckVersion(
            currentVersion,
            messageVersion,
            onSuccess: () => handler.Handle(@event, context.CancellationToken),
            onFailure: () => logger.LogWarning(
                "Skipping UserUpsertedV1 event for UserId: {UserId} due to version conflict. " +
                "Event EntityVersion: {EventEntityVersion}, Current EntityVersion: {CurrentEntityVersion}",
                message.UserId,
                message.EntityVersion,
                user?.EntityVersion));
    }
}

public static class VersionChecker
{
    public static Task CheckVersion(
        long currentVersion, 
        long eventVersion, 
        Func<Task> onSuccess,
        Action? onFailure = null)
    {
        if (currentVersion == eventVersion - 1)
        {
            return onSuccess();
        }

        onFailure?.Invoke();

        throw new InvalidOperationException(
            $"Event v{eventVersion}, Current v{currentVersion}");
    }
}
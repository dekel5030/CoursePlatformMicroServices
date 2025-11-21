using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Primitives;
using Kernel;
using Microsoft.Extensions.Logging;

namespace Application.Users.IntegrationEvents.AuthRegistered;

public class AuthRegisteredIntegrationEventHandler(
    IWriteDbContext dbContext,
    ILogger<AuthRegisteredIntegrationEventHandler> logger) : IIntegrationEventHandler<AuthRegisteredIntegrationEvent>
{
    public Task Handle(
        AuthRegisteredIntegrationEvent request, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Handling AuthRegisteredIntegrationEvent for AuthUserId: {AuthUserId}, UserId: {UserId}, Email: {Email}, Username: {Username}",
            request.AuthUserId,
            request.UserId,
            request.Email,
            request.Username ?? "N/A");

        AuthUserId authUserId = new AuthUserId(request.AuthUserId);
        UserId userId = new UserId(Guid.Parse(request.UserId));

        User? user = dbContext.Users
            .FirstOrDefault(u => u.AuthUserId == authUserId);

        if (user is null)
        {
            logger.LogInformation(
                "No existing user found with AuthUserId: {AuthUserId}. Creating new user with UserId: {UserId}.",
                request.AuthUserId,
                request.UserId);

            Result<User> newUserResult = User.CreateUser(
                authUserId,
                request.Email,
                userId); // Pass the userId so it matches authUserId

            if (newUserResult.IsFailure)
            {
                logger.LogError(
                    "Failed to create user for AuthUserId: {AuthUserId}, Email: {Email}. Error: {Error}",
                    request.AuthUserId,
                    request.Email,
                    newUserResult.Error);

                return Task.CompletedTask;
            }

            dbContext.Users.Add(newUserResult.Value);
            return dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            logger.LogInformation(
                "User with AuthUserId: {AuthUserId} already exists. No action taken.",
                request.AuthUserId);
            return Task.CompletedTask;
        }
    }
}

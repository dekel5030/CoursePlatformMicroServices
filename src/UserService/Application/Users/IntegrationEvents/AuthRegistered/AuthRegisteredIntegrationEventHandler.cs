using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
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
            "Handling AuthRegisteredIntegrationEvent for Email: {Email}, Username: {Username}",
            request.Email,
            request.Username ?? "N/A");

        User? user = dbContext.Users
            .FirstOrDefault(u => u.Email == request.Email);

        if (user is null)
        {
            logger.LogInformation(
                "No existing user found with Email: {Email}. Creating new user.",
                request.Email);

            Result<User> newUserResult = User.CreateUser(request.Email);

            if (newUserResult.IsFailure)
            {
                logger.LogError(
                    "Failed to create user for Email: {Email}. Error: {Error}",
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
                "User with Email: {Email} already exists. No action taken.",
                request.Email);
            return Task.CompletedTask;
        }
    }
}
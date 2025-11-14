using Application.Abstractions.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users.Contracts.Events;

namespace Infrastructure.MassTransit.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedV1>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext,
        ILogger<UserCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedV1> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received UserCreatedV1 event: AuthUserId={AuthUserId}, UserId={UserId}",
            message.AuthUserId,
            message.UserId);

        // Parse the AuthUserId from string to Guid
        if (!Guid.TryParse(message.AuthUserId, out var authUserIdGuid))
        {
            _logger.LogError("Invalid AuthUserId format: {AuthUserId}", message.AuthUserId);
            throw new InvalidOperationException($"Invalid AuthUserId format: {message.AuthUserId}");
        }

        var authUserId = new Domain.AuthUsers.Primitives.AuthUserId(authUserIdGuid);

        // Find the auth user
        var authUser = await _readDbContext.AuthUsers
            .FirstOrDefaultAsync(u => u.Id == authUserId, context.CancellationToken);

        if (authUser == null)
        {
            _logger.LogWarning("AuthUser not found with Id: {AuthUserId}", authUserId.Value);
            throw new InvalidOperationException($"AuthUser not found with Id: {authUserId.Value}");
        }

        // Link the userId
        authUser.LinkUserId(message.UserId);

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation(
            "Successfully linked UserId {UserId} to AuthUser {AuthUserId}",
            message.UserId,
            message.AuthUserId);
    }
}

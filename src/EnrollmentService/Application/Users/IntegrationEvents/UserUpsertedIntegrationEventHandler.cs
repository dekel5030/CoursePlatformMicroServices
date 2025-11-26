using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.IntegrationEvents;

public sealed class UserUpsertedIntegrationEventHandler
    : IIntegrationEventHandler<UserUpsertedIntegrationEvent>
{
    private readonly IWriteDbContext _dbContext;
    private readonly ILogger<UserUpsertedIntegrationEventHandler> _logger;

    public UserUpsertedIntegrationEventHandler(
        IWriteDbContext dbContext,
        ILogger<UserUpsertedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(
        UserUpsertedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var userId = new ExternalUserId(integrationEvent.UserId);

        var existingUser = await _dbContext.KnownUsers
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (existingUser is null)
        {
            var newUser = KnownUser.Create(userId, integrationEvent.Name, integrationEvent.IsActive);
            _dbContext.KnownUsers.Add(newUser);
            _logger.LogInformation("Created KnownUser for UserId: {UserId}", integrationEvent.UserId);
        }
        else
        {
            existingUser.Update(integrationEvent.Name, integrationEvent.IsActive);
            _logger.LogInformation("Updated KnownUser for UserId: {UserId}", integrationEvent.UserId);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

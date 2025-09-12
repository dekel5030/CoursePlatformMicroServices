using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.IntegrationEvents.UserUpserted;

public sealed class UserUpsertedIntegrationEventHandler(IWriteDbContext dbContext)
    : IIntegrationEventHandler<UserUpsertedIntegrationEvent>
{
    public async Task Handle(
        UserUpsertedIntegrationEvent request, 
        CancellationToken cancellationToken = default)
    {
        ExternalUserId externalUserId = new ExternalUserId(request.UserId);

        User? user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.ExternalUserId == externalUserId, cancellationToken);

        if (user is null)
        {
            user = User.Create(
                externalUserId: externalUserId,
                email: request.Email,
                fullname: request.Fullname,
                isActive: request.IsActive);

            await dbContext.Users.AddAsync(user, cancellationToken);
        }
        else
        {
            user.Fullname = request.Fullname;
            user.Email = request.Email;
            user.IsActive = request.IsActive;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


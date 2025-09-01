using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.IntegrationEvents.UserUpserted;

public sealed class UserUpsertedIntegrationEventHandler(IApplicationDbContext dbContext)
    : IIntegrationEventHandler<UserUpsertedIntegrationEvent>
{
    public async Task Handle(
        UserUpsertedIntegrationEvent request, 
        CancellationToken cancellationToken = default)
    {
        User? user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.ExternalUserId == request.UserId, cancellationToken);

        if (user is null)
        {
            user = User.Create(
                externalUserId: request.UserId,
                email: request.Email,
                fullname: request.Fullname,
                isActive: request.IsActive);

            await dbContext.Users.AddAsync(user, cancellationToken);
        }
        else // if (user.Version < request.AggregateVersion)
        {
            user.Fullname = request.Fullname;
            user.Email = request.Email;
            user.IsActive = request.IsActive;
        }
        //else
        //{
        //    return;
        //}

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


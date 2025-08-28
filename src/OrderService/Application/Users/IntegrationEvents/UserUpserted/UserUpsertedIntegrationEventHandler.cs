using Application.Abstractions.Messaging;
using Kernel;

namespace Application.Users.IntegrationEvents.UserUpserted;

public sealed class UserUpsertedIntegrationEventHandler
    : IIntegrationEventHandler<UserUpsertedIntegrationEvent>
{
    public Task<Result> Handle(
        UserUpsertedIntegrationEvent request, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}


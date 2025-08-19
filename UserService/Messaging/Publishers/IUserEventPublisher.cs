using Common.Messaging.EventEnvelope;
using Users.Contracts.Events;

namespace UserService.Messaging.Publishers;

public interface IUserEventPublisher
{
    Task PublishUserUpsertedAsync(EventEnvelope<UserUpsertedV1> envelope, CancellationToken ct = default);
}
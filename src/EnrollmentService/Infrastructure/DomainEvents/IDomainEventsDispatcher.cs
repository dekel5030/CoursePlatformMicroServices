using SharedKernel;

namespace Infrastructure.DomainEvents;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<Entity> entities, CancellationToken cancellationToken = default);
}

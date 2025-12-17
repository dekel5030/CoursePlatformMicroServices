using SharedKernel;

namespace Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
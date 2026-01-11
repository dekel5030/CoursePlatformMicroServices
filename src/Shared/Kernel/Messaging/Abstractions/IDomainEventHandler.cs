namespace Kernel.Messaging.Abstractions;

public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : class, IDomainEvent
{
}

namespace Kernel.Messaging.Abstractions;

public interface IDomainEventHandler<TEvent> : IEventHandler<TEvent>
    where TEvent : class, IDomainEvent
{
}
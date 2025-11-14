using SharedKernel;

namespace Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent> : IHandler<TEvent, Task>
    where TEvent : IDomainEvent
{ }

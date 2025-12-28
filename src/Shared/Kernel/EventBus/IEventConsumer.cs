using Kernel.Messaging.Abstractions;

namespace Kernel.EventBus;

public interface IEventConsumer<TEvent> : IEventHandler<TEvent>
    where TEvent : class
{

}
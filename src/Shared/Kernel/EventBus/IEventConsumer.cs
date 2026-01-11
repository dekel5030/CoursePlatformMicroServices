using Kernel.Messaging.Abstractions;

namespace Kernel.EventBus;

public interface IEventConsumer<in TEvent> : IEventHandler<TEvent>
    where TEvent : class
{

}

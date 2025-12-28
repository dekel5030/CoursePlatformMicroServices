namespace Kernel.EventBus;

public interface IEventConsumer<TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

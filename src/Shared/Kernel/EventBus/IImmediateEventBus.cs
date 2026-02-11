namespace Kernel.EventBus;

public interface IImmediateEventBus
{
    Task PublishAsync<TEvent>(TEvent message, CancellationToken cancellationToken = default)
        where TEvent : class;
}

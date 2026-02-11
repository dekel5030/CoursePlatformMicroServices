using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Courses.Infrastructure.MassTransit;

internal sealed class ImmediateEventBus : IImmediateEventBus
{
    private readonly IBus _bus;
    private readonly ILogger<ImmediateEventBus> _logger;

    public ImmediateEventBus(IBus bus, ILogger<ImmediateEventBus> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent message, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        _logger.LogDebug("Publishing event immediately (no outbox): {EventType}", typeof(TEvent).FullName);
        return _bus.Publish(message, cancellationToken);
    }
}

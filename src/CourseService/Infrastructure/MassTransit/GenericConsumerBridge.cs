using Kernel.Messaging.Abstractions;
using MassTransit;

namespace Courses.Infrastructure.MassTransit;

public class GenericConsumerBridge<TEvent> : IConsumer<TEvent>
    where TEvent : class
{
    private readonly IMediator _mediator;

    public GenericConsumerBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Consume(ConsumeContext<TEvent> context)
    {
        return _mediator.Publish(context.Message, context.CancellationToken);
    }
}
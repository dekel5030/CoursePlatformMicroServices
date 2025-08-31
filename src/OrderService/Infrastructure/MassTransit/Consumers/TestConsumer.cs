using Domain.Orders.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MassTransit.Consumers;

public class TestConsumer(ILogger<TestConsumer> logger) : IConsumer<OrderSubmitted>
{
    public Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        logger.LogError("Message Consumed"); 
        return Task.CompletedTask;
    }
}
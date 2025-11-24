using Application.Courses.IntegrationEvents;
using Courses.Contracts.Events;
using MassTransit;

namespace Infrastructure.MassTransit.Consumers;

public class CourseUpsertedConsumer : IConsumer<CourseUpsertedV1>
{
    private readonly Application.Abstractions.Messaging.IIntegrationEventHandler<CourseUpsertedIntegrationEvent> _handler;

    public CourseUpsertedConsumer(
        Application.Abstractions.Messaging.IIntegrationEventHandler<CourseUpsertedIntegrationEvent> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<CourseUpsertedV1> context)
    {
        var integrationEvent = new CourseUpsertedIntegrationEvent(
            context.Message.CourseId,
            Title: "", // CourseUpsertedV1 doesn't have title
            context.Message.IsPublished);

        await _handler.Handle(integrationEvent, context.CancellationToken);
    }
}

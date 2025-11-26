using Application.Abstractions.Messaging;
using Application.Courses.IntegrationEvents;
using Courses.Contracts.Events;
using MassTransit;

namespace Infrastructure.MassTransit.Consumers;

public class CourseUpsertedConsumer : IConsumer<CourseUpserted>
{
    private readonly IIntegrationEventHandler<CourseUpsertedIntegrationEvent> _handler;

    public CourseUpsertedConsumer(
        IIntegrationEventHandler<CourseUpsertedIntegrationEvent> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<CourseUpserted> context)
    {
        var integrationEvent = new CourseUpsertedIntegrationEvent(
            context.Message.CourseId,
            Title: context.Message.Title,
            context.Message.IsPublished);

        await _handler.Handle(integrationEvent, context.CancellationToken);
    }
}
using Common.Messaging;
using Common.Messaging.EventEnvelope;
using Enrollments.Contracts.Events;
using Enrollments.Contracts.Routing;
using MassTransit;

namespace EnrollmentService.Messaging.Publishers;

public sealed class EnrollmentEventPublisher : IEnrollmentEventPublisher
{
    private readonly IPublishEndpoint _publish;

    public EnrollmentEventPublisher(IPublishEndpoint publish) => _publish = publish;

    public async Task PublishEnrollmentUpsertedAsync(
        EventEnvelope<EnrollmentUpsertedV1> envelope, CancellationToken ct = default)
    {
        await _publish.Publish(envelope, ctx =>
        {
            ctx.SetRoutingKey(RoutingKeys.Upserted(EnrollmentUpsertedV1.Version));
            ctx.Headers.Set(HeaderNames.CorrelationId, envelope.CorrelationId);
        }, ct);
    }
}

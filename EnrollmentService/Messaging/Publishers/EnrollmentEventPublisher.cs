using Common.Messaging;
using Common.Messaging.EventEnvelope;
using Enrollments.Contracts.Events;
using Enrollments.Contracts.ReasonCodes;
using Enrollments.Contracts.Routing;
using MassTransit;

namespace EnrollmentService.Messaging.Publishers;

public sealed class EnrollmentEventPublisher : IEnrollmentEventPublisher
{
    private readonly string _source = "EnrollmentService";
    private readonly IPublishEndpoint _publish;

    public EnrollmentEventPublisher(IPublishEndpoint publish) => _publish = publish;

    public async Task PublishEnrollmentCreatedAsync(
        int enrollmentId, int userId, int courseId, string correlationId, CancellationToken ct = default)
    {
        var message = new EnrollmentCreatedV1(
            EnrollmentId: enrollmentId,
            UserId: userId,
            CourseId: courseId
        );

        int version = EnrollmentCreatedV1.Version;
        string eventType = EnrollmentCreatedV1.EventType;

        var envelope = CreateEnvelope(correlationId, message, version, eventType);

        await _publish.Publish(envelope, ctx =>
        {
            ctx.SetRoutingKey(RoutingKeys.Created(version));
            ctx.Headers.Set(HeaderNames.CorrelationId, correlationId);
        }, ct);
    }

    public async Task PublishEnrollmentCancelledAsync(
        int enrollmentId, int userId, int courseId, string correlationId,
        CancellationReasons reason, CancellationToken ct = default)
    {
        var message = new EnrollmentCancelledV1(
            EnrollmentId: enrollmentId,
            UserId: userId,
            CourseId: courseId,
            ReasonCode: CancellationReasonMapper.ToContractCode(reason)
        );

        int version = EnrollmentCancelledV1.Version;
        string eventType = EnrollmentCancelledV1.EventType;

        var envelope = CreateEnvelope(correlationId, message, version, eventType);

        await _publish.Publish(envelope, ctx =>
        {
            ctx.SetRoutingKey(RoutingKeys.Cancelled(version));
            ctx.Headers.Set(HeaderNames.CorrelationId, correlationId);
        }, ct);
    }

    private EventEnvelope<T> CreateEnvelope<T>(string correlationId, T payload, int version, string eventType)
    {
        return new EventEnvelope<T>
        (
            EventId: Guid.NewGuid(),
            CorrelationId: correlationId,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            Source: _source,
            EventType: eventType,
            Version: version,
            Payload: payload
        );
    }

    public enum CancellationReasons
    {
        Deleted,
        Expired,
        UserRequested
    }

    internal static class CancellationReasonMapper
    {
        public static string ToContractCode(CancellationReasons reason) => reason switch
        {
            CancellationReasons.Deleted => EnrollmentCancellationReasonCodes.Deleted,
            CancellationReasons.Expired => EnrollmentCancellationReasonCodes.Expired,
            CancellationReasons.UserRequested => EnrollmentCancellationReasonCodes.UserRequested,
            _ => "Unknown"
        };
    }
}

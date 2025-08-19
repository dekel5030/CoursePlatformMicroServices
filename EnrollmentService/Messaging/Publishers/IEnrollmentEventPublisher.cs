using Common.Messaging.EventEnvelope;
using Enrollments.Contracts.Events;

namespace EnrollmentService.Messaging.Publishers;

public interface IEnrollmentEventPublisher
{
    Task PublishEnrollmentUpsertedAsync(
        EventEnvelope<EnrollmentUpsertedV1> envelope, CancellationToken ct = default);
}

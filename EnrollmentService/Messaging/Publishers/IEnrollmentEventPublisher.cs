using static EnrollmentService.Messaging.Publishers.EnrollmentEventPublisher;

namespace EnrollmentService.Messaging.Publishers;

public interface IEnrollmentEventPublisher
{
    Task PublishEnrollmentCreatedAsync(
        int enrollmentId, int userId, int courseId, string correlationId, CancellationToken ct = default);
    Task PublishEnrollmentCancelledAsync(
        int enrollmentId, int userId, int courseId, string correlationId,
        CancellationReasons reason, CancellationToken ct = default);
}

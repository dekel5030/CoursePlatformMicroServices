using Enrollments.Contracts.Events;
using MassTransit;

namespace EnrollmentService.Messaging.Publishers;

public sealed class EnrollmentEventPublisher : IEnrollmentEventPublisher
{
    private readonly IPublishEndpoint _publish;

    public EnrollmentEventPublisher(IPublishEndpoint publish) => _publish = publish;

    public async Task PublishEnrollmentCreatedAsync(int enrollmentId, int userId, int courseId, Guid correlationId)
    {
        var msg = new EnrollmentCreatedV1(
            MessageId: Guid.NewGuid(),
            CorrelationId: correlationId,
            EnrollmentId: enrollmentId,
            UserId: userId,
            CourseId: courseId,
            EnrolledAtUtc: DateTime.UtcNow
        );

        await _publish.Publish(msg, ctx => ctx.Headers.Set("schema-version", 1));
    }
}

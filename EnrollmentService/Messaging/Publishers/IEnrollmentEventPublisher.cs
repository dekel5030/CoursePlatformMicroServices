namespace EnrollmentService.Messaging.Publishers;

public interface IEnrollmentEventPublisher
{
    Task PublishEnrollmentCreatedAsync(int enrollmentId, int userId, int courseId, Guid correlationId);
}

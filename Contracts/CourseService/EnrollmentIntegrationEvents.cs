namespace CoursePlatform.Contracts.CourseService;

public sealed record EnrollmentCreatedIntegrationEvent(
    Guid EnrollmentId,
    Guid CourseId,
    Guid StudentId,
    DateTimeOffset EnrolledAt);

public sealed record LessonCompletedIntegrationEvent(
    Guid EnrollmentId,
    Guid CourseId,
    Guid StudentId,
    Guid LessonId,
    bool IsCourseComplete);

public sealed record EnrollmentStatusChangedIntegrationEvent(
    Guid EnrollmentId,
    Guid CourseId,
    string NewStatus);
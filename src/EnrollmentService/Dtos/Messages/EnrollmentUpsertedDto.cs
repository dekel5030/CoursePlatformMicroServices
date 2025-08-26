namespace EnrollmentService.Dtos.Messages;

public sealed record EnrollmentUpsertedDto (
    int EnrollmentId,
    int UserId,
    int CourseId,
    bool IsActive
);
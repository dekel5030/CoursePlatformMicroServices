using Domain.Enrollments.Errors;
using Domain.Enrollments.Events;
using Domain.Enrollments.Primitives;
using Domain.Users.Primitives;
using Domain.Courses.Primitives;
using Kernel;
using SharedKernel;

namespace Domain.Enrollments;

public class Enrollment : Entity, IVersionedEntity
{
    private Enrollment() { }

    public EnrollmentId Id { get; private set; }
    public ExternalUserId UserId { get; private set; }
    public ExternalCourseId CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public long EntityVersion { get; private set; }

    public static Enrollment Create(
        ExternalUserId userId,
        ExternalCourseId courseId,
        DateTime? expiresAt = null)
    {
        var enrollment = new Enrollment
        {
            Id = new EnrollmentId(Guid.CreateVersion7()),
            UserId = userId,
            CourseId = courseId,
            Status = EnrollmentStatus.Pending,
            EnrolledAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            EntityVersion = 1
        };

        enrollment.Raise(new EnrollmentCreatedDomainEvent(
            enrollment.Id,
            userId,
            courseId,
            enrollment.Status,
            enrollment.EnrolledAt));

        return enrollment;
    }

    public Result Activate()
    {
        if (Status == EnrollmentStatus.Cancelled)
            return Result.Failure(EnrollmentErrors.EnrollmentAlreadyCancelled);

        if (Status == EnrollmentStatus.Completed)
            return Result.Failure(EnrollmentErrors.EnrollmentAlreadyCompleted);

        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
            return Result.Failure(EnrollmentErrors.EnrollmentExpired);

        var oldStatus = Status;
        Status = EnrollmentStatus.Active;

        Raise(new EnrollmentStatusChangedDomainEvent(Id, oldStatus, Status));

        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != EnrollmentStatus.Active)
            return Result.Failure(new Error(
                "Enrollment.CannotComplete",
                "Only active enrollments can be completed",
                ErrorType.Validation));

        var oldStatus = Status;
        Status = EnrollmentStatus.Completed;

        Raise(new EnrollmentStatusChangedDomainEvent(Id, oldStatus, Status));

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == EnrollmentStatus.Cancelled)
            return Result.Failure(EnrollmentErrors.EnrollmentAlreadyCancelled);

        if (Status == EnrollmentStatus.Completed)
            return Result.Failure(EnrollmentErrors.EnrollmentAlreadyCompleted);

        var oldStatus = Status;
        Status = EnrollmentStatus.Cancelled;

        Raise(new EnrollmentStatusChangedDomainEvent(Id, oldStatus, Status));

        return Result.Success();
    }
}

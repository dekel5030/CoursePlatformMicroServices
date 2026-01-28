using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Errors;
using Courses.Domain.Enrollments.Events;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared;
using Kernel;

namespace Courses.Domain.Enrollments;

public interface IEnrollmentSnapshot
{
    EnrollmentId Id { get; }
    CourseId CourseId { get; }
    UserId StudentId { get; }
    DateTimeOffset EnrolledAt { get; }
    DateTimeOffset ExpiresAt { get; }
    EnrollmentStatus Status { get; }
    DateTimeOffset? CompletedAt { get; }
    LessonId? LastAccessedLessonId { get; }
    DateTimeOffset? LastAccessedAt { get; }
    IReadOnlySet<LessonId> CompletedLessons { get; }
}

public class Enrollment : Entity<EnrollmentId>, IEnrollmentSnapshot
{
    public override EnrollmentId Id { get; protected set; }
    public CourseId CourseId { get; private set; }
    public UserId StudentId { get; private set; }
    public DateTimeOffset EnrolledAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }
    public bool IsCompleted => CompletedAt.HasValue;

    public LessonId? LastAccessedLessonId { get; private set; }
    public DateTimeOffset? LastAccessedAt { get; private set; }

    public IReadOnlySet<LessonId> CompletedLessons => _completedLessons;
    private readonly HashSet<LessonId> _completedLessons = new();
    
    public bool IsFullyCompleted(int totalLessonsInCourse)
        => totalLessonsInCourse > 0 && _completedLessons.Count == totalLessonsInCourse;

    private Enrollment(EnrollmentId id, CourseId courseId, UserId studentId)
    {
        Id = id;
        CourseId = courseId;
        StudentId = studentId;
    }

    public static Result<Enrollment> Create(
        CourseId courseId,
        UserId studentId,
        DateTimeOffset enrolledAt,
        DateTimeOffset expiresAt)
    {
        if (expiresAt <= enrolledAt)
        {
            return Result.Failure<Enrollment>(EnrollmentErrors.InvalidExpirationDate);
        }

        var enrollment = new Enrollment(EnrollmentId.CreateNew(), courseId, studentId)
        {
            EnrolledAt = enrolledAt,
            ExpiresAt = expiresAt,
            Status = EnrollmentStatus.Active
        };

        enrollment.Raise(new EnrollmentCreatedDomainEvent(enrollment));

        return Result.Success(enrollment);
    }

    public void MarkLessonAsCompleted(LessonId lessonId, int totalLessonsInCourse)
    {
        if (Status != EnrollmentStatus.Active)
        {
            return;
        }

        LastAccessedLessonId = lessonId;
        LastAccessedAt = DateTimeOffset.UtcNow;

        if (!_completedLessons.Add(lessonId))
        {
            return;
        }

        if (totalLessonsInCourse > 0 && _completedLessons.Count == totalLessonsInCourse && !IsCompleted)
        {
            CompletedAt = DateTimeOffset.UtcNow;
        }

        Raise(new EnrollmentUpdatedDomainEvent(this));
    }

    public void Expire()
    {
        Status = EnrollmentStatus.Expired;

        Raise(new EnrollmentStatusChangedDomainEvent(this));
    }

    public void Revoke()
    {
        Status = EnrollmentStatus.Revoked;

        Raise(new EnrollmentStatusChangedDomainEvent(this));
    }
}

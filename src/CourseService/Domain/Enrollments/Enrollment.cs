using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared;
using Kernel;

namespace Courses.Domain.Enrollments;

public class Enrollment : Entity<EnrollmentId>
{
    public override EnrollmentId Id { get; protected set; } = EnrollmentId.CreateNew();
    public CourseId CourseId { get; private set; }
    public UserId StudentId { get; private set; }
    public DateTimeOffset EnrolledAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    public float ProgressPercentage { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public LessonId? LastAccessedLessonId { get; private set; }
    public DateTimeOffset? LastAccessedAt { get; private set; }

    public IReadOnlySet<LessonId> CompletedLessons => _completedLessons;

    private readonly HashSet<LessonId> _completedLessons = new();

#pragma warning disable S1133
#pragma warning disable CS8618
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private Enrollment() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    private Enrollment(EnrollmentId id, CourseId courseId, UserId studentId)
    {
        Id = id;
        CourseId = courseId;
        StudentId = studentId;
    }

    internal static Result<Enrollment> Create(
        CourseId courseId,
        UserId studentId,
        DateTimeOffset enrolledAt,
        DateTimeOffset expiresAt)
    {
        var id = EnrollmentId.CreateNew();

        var enrollment = new Enrollment(id, courseId, studentId)
        {
            EnrolledAt = enrolledAt,
            ExpiresAt = expiresAt,
            Status = EnrollmentStatus.Active
        };

        return Result.Success(enrollment);
    }

    public void MarkLessonAsCompleted(LessonId lessonId, int totalLessonsInCourse)
    {
        LastAccessedLessonId = lessonId;
        LastAccessedAt = DateTimeOffset.UtcNow;

        if (!_completedLessons.Add(lessonId))
        {
            return;
        }

        if (totalLessonsInCourse > 0)
        {
            ProgressPercentage = (float)_completedLessons.Count / totalLessonsInCourse * 100;
        }

        if (_completedLessons.Count == totalLessonsInCourse && CompletedAt is null)
        {
            CompletedAt = DateTimeOffset.UtcNow;
        }
    }
}

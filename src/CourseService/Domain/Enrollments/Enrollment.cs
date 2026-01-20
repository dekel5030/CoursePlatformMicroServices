using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Kernel;

namespace Courses.Domain.Enrollments;

public class Enrollment
{
    public EnrollmentId Id { get; private set; } = EnrollmentId.CreateNew();
    public CourseId CourseId { get; private set; }
    public UserId StudentId { get; private set; }
    public DateTimeOffset EnrolledAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public EnrollmentStatus Status { get; private set; }

#pragma warning disable CS8618
    private Enrollment() { }
#pragma warning restore CS8618

    internal static Result<Enrollment> Create(
        CourseId courseId,
        UserId studentId,
        TimeProvider timeProvider,
        TimeSpan? validFor = null)
    {
        DateTimeOffset now = timeProvider.GetUtcNow();
        TimeSpan duration = validFor ?? TimeSpan.FromDays(365);

        var enrollment = new Enrollment()
        {
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAtUtc = now,
            ExpiresAtUtc = now.Add(duration),
            Status = EnrollmentStatus.Active
        };

        return Result.Success(enrollment);
    }
}

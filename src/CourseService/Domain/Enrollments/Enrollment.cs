using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;

namespace Courses.Domain.Enrollments;

public class Enrollment
{
    public EnrollmentId Id { get; private set; } = EnrollmentId.CreateNew();
    public CourseId CourseId { get; private set; }
    public StudentId StudentId { get; private set; }
    public DateTimeOffset EnrolledAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }

    #pragma warning disable CS8618
    private Enrollment() { }
    #pragma warning restore CS8618

    internal static Enrollment Create(
        CourseId courseId,
        StudentId studentId,
        TimeProvider timeProvider,
        TimeSpan? validFor = null)
    {
        DateTimeOffset now = timeProvider.GetUtcNow();
        TimeSpan duration = validFor ?? TimeSpan.FromDays(365);

        return new Enrollment()
        {
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAtUtc = now,
            ExpiresAtUtc = now.Add(duration)
        };
    }
}

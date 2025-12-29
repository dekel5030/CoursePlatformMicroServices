using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;

namespace Courses.Domain.Enrollments;

public class Enrollment
{
    public EnrollmentId Id { get; private set; } = EnrollmentId.CreateNew();
    public CourseId CourseId { get; private set; }
    public StudentId StudentId { get; private set; }
    public DateTimeOffset EnrolledAtUtc { get; private set; }


    #pragma warning disable CS8618
    private Enrollment() { }
    #pragma warning restore CS8618

    internal static Enrollment CreateEnrollment(
        CourseId courseId,
        StudentId studentId,
        TimeProvider timeProvider)
    {
        return new Enrollment
        {
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAtUtc = timeProvider.GetUtcNow()
        };
    }
}

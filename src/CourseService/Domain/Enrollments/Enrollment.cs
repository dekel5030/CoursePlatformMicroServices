using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Kernel;

namespace Courses.Domain.Enrollments;

public class Enrollment
{
    public EnrollmentId Id { get; private set; } = EnrollmentId.CreateNew();
    public CourseId CourseId { get; private set; }
    public StudentId StudentId { get; private set; }
    public DateTimeOffset EnrolledAtUtc { get; private set; }
    public TimeSpan ValidFor { get; private set; }

    #pragma warning disable CS8618
    private Enrollment() { }
    #pragma warning restore CS8618

    internal static Enrollment CreateEnrollment(
        CourseId courseId,
        StudentId studentId,
        TimeProvider timeProvider,
        TimeSpan? validFor = null)
    {
        return new Enrollment()
        {
            CourseId = courseId.Id,
            StudentId = studentId,
            EnrolledAtUtc = timeProvider.GetUtcNow(),
            ValidFor = validFor ?? TimeSpan.FromDays(365)
        };
    }
}

public static class EnrollmentErrors
{
    public static readonly Error UnPublishedCourse = Error.Validation(
        "Enrollment.CourseUnpublished",
        "Cannot enroll in unpublished course");
}
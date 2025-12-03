using Domain.Courses.Primitives;
using Domain.Enrollments.Events;
using Domain.Enrollments.Primitives;
using SharedKernel;

namespace Domain.Enrollments;

public class Enrollment : Entity
{
    private Enrollment() { }

    public EnrollmentId Id { get; private set; }
    public Guid StudentId { get; private set; }
    public CourseId CourseId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public CompletionStatus CompletionStatus { get; private set; }

    public static Enrollment Create(
        Guid studentId,
        CourseId courseId)
    {
        var enrollment = new Enrollment
        {
            Id = new EnrollmentId(Guid.CreateVersion7()),
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            CompletionStatus = CompletionStatus.NotStarted
        };

        enrollment.Raise(new StudentEnrolledDomainEvent(
            enrollment.Id,
            studentId,
            courseId,
            enrollment.EnrollmentDate));

        return enrollment;
    }
}

using Courses.Domain.Courses.Enrollments;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public class EnrollmentManager
{
    private readonly TimeProvider _timeProvider;

    public EnrollmentManager(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Result<Enrollment> EnrollStudent(
        Course course,
        UserId studentId,
        TimeSpan validFor)
    {
        Result<Enrollment> enrollmentResult = Enrollment.Create(
            course.Id,
            studentId,
            _timeProvider,
            validFor);

        if (enrollmentResult.IsFailure)
        {
            return Result.Failure<Enrollment>(enrollmentResult.Error);
        }

        Result courseEnrollResult = course.Enroll();

        if (courseEnrollResult.IsFailure)
        {
            return Result.Failure<Enrollment>(courseEnrollResult.Error);
        }

        return Result.Success(enrollmentResult.Value);
    }
}

using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Domain.Enrollments;

public class EnrollmentManager
{
    private readonly TimeProvider _timeProvider;

    public EnrollmentManager(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Result<Enrollment> EnrollUser(
        Course course,
        UserId studentId,
        TimeSpan validFor)
    {
        DateTimeOffset currentTime = _timeProvider.GetUtcNow();
        Result<Enrollment> enrollmentResult = Enrollment.Create(
            course.Id,
            studentId,
            currentTime,
            currentTime + validFor);

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

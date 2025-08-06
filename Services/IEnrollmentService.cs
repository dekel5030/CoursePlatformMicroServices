using Common;

namespace CourseService.Services;

public interface IEnrollmentService
{
    Task<Result<bool>> IsUserEnrolledInCourseAsync(int courseId, int userId);
}
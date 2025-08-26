namespace EnrollmentService.Data.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<bool> CourseExistsAsync(int courseId, CancellationToken ct = default);
}
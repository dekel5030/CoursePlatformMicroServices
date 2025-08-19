using CourseService.Models;

namespace CourseService.Data.EnrollmentsRepo;

public interface IEnrollmentsRepo
{
    Task AddAsync(Enrollment newEnrollment, CancellationToken ct = default);
    Task<Enrollment?> GetByIdAsync(int enrollmentId, CancellationToken ct = default);
}
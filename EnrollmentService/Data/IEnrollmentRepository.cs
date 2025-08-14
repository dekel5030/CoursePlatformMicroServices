using EnrollmentService.Data.Queries.Interfaces;
using EnrollmentService.Models;

namespace EnrollmentService.Data;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(IReadOnlyList<Enrollment> enrollments, int totalCount)> SearchEnrollmentsAsync(
        IQueryObject<Enrollment> query, CancellationToken ct = default);
    Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
    void Remove(Enrollment enrollment);

    Task<bool> ExistsAsync(int courseId, int userId, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
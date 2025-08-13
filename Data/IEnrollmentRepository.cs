using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.Data;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(IEnumerable<Enrollment> enrollments, int totalCount)> SearchEnrollmentsAsync(EnrollmentSearchDto searchDto, CancellationToken ct = default);
    Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
    void Remove(Enrollment enrollment);

    Task<bool> Exists(int courseId, int userId, CancellationToken ct = default);

    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}
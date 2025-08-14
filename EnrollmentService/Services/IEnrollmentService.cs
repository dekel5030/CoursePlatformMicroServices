using Common;
using EnrollmentService.Dtos;

namespace EnrollmentService.Services;

public interface IEnrollmentService
{
    Task<Result<EnrollmentReadDto>> GetEnrollmentByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResponseDto<EnrollmentReadDto>> SearchEnrollmentsAsync(EnrollmentSearchDto searchDto, CancellationToken ct = default);
    Task<Result<EnrollmentReadDto>> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentCreateDto, CancellationToken ct = default);
    Task<Result<bool>> DeleteEnrollmentAsync(int id, CancellationToken ct = default);
}
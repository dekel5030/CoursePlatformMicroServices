using Common;
using EnrollmentService.Dtos;

namespace EnrollmentService.Services;

public interface IEnrollmentService
{
    Task<PagedResponseDto<EnrollmentReadDto>> SearchEnrollmentsAsync(EnrollmentSearchDto searchDto);
    Task<Result<EnrollmentReadDto>> GetEnrollmentByIdAsync(int id);
    Task<Result<EnrollmentReadDto>> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentCreateDto);
    Task<Result<bool>> DeleteEnrollmentAsync(int id);
}
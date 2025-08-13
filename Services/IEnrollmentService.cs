using Common;

namespace EnrollmentService.Services;

public interface IEnrollmentService
{
    Task<PagedResponseDto<EnrollmentReadDto>> SearchEnrollmentsAsync();
    Task<Result<EnrollmentReadDto>> GetEnrollmentByIdAsync(int id);
    Task<Result<EnrollmentReadDto>> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentCreateDto);
    Task<Result<bool>> DeleteEnrollmentAsync(int id);
}
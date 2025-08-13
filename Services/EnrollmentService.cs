using Common;
using Common.Errors;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IMapper _mapper;

    public EnrollmentService(IEnrollmentRepository enrollmentRepository, IMapper mapper)
    {
        _enrollmentRepo = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<EnrollmentReadDto>> GetEnrollmentByIdAsync(int id)
    {
        Enrollment? enrollment = await _enrollmentRepo.GetByIdAsync(id);

        if (enrollment is null)
        {
            return Result<EnrollmentReadDto>.Failure(Error.EnrollmentNotFound);
        }

        var enrollmentReadDto = _mapper.Map<EnrollmentReadDto>(enrollment);

        return Result<EnrollmentReadDto>.Success(enrollmentReadDto);
    }

    public async Task<PagedResponseDto<EnrollmentReadDto>> SearchEnrollmentsAsync(EnrollmentSearchDto searchDto)
    {
        (IEnumerable<Enrollment> enrollments, int totalCount) = await _enrollmentRepo.SearchEnrollmentsAsync(searchDto);

        var enrollmentReadDtos = _mapper.Map<IEnumerable<EnrollmentReadDto>>(enrollments);

        return new PagedResponseDto<EnrollmentReadDto>
        {
            Items = enrollmentReadDtos,
            TotalCount = totalCount,
            PageSize = searchDto.PageSize,
            PageNumber = searchDto.PageNumber
        };
    }

    public async Task<Result<EnrollmentReadDto>> CreateEnrollmentAsync(EnrollmentCreateDto CreateDto)
    {
        if (await _enrollmentRepo.Exists(CreateDto.CourseId, CreateDto.UserId))
        {
            return Result<EnrollmentReadDto>.Failure(Error.EnrollmentAlreadyExists);
        }

        var enrollment = _mapper.Map<Enrollment>(CreateDto);

        await _enrollmentRepo.AddAsync(enrollment);
        await _enrollmentRepo.SaveChangesAsync();

        var enrollmentReadDto = _mapper.Map<EnrollmentReadDto>(enrollment);
        return Result<EnrollmentReadDto>.Success(enrollmentReadDto);
    }

    public async Task<Result<bool>> DeleteEnrollmentAsync(int id)
    {
        Enrollment? enrollment = await _enrollmentRepo.GetByIdAsync(id);

        if (enrollment is null)
        {
            return Result<bool>.Failure(Error.EnrollmentNotFound);
        }

        _enrollmentRepo.Remove(enrollment);
        await _enrollmentRepo.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
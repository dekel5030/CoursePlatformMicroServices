using Common;
using Common.Errors;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using Microsoft.Extensions.Logging;

namespace EnrollmentService.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper,
        ILogger<EnrollmentService> logger)
    {
        _enrollmentRepo = enrollmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<EnrollmentReadDto>> GetEnrollmentByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        Enrollment? enrollment = await _enrollmentRepo.GetByIdAsync(id, ct);

        if (enrollment is null)
        {
            _logger.LogInformation("Enrollment {EnrollmentId} not found.", id);
            return Result<EnrollmentReadDto>.Failure(Error.EnrollmentNotFound);
        }

        var enrollmentReadDto = _mapper.Map<EnrollmentReadDto>(enrollment);

        return Result<EnrollmentReadDto>.Success(enrollmentReadDto);
    }

    public async Task<PagedResponseDto<EnrollmentReadDto>> SearchEnrollmentsAsync(
        EnrollmentSearchDto searchDto,
        CancellationToken ct = default)
    {
        (IEnumerable<Enrollment> enrollments, int totalCount) =
            await _enrollmentRepo.SearchEnrollmentsAsync(searchDto, ct);

        var enrollmentReadDtos = _mapper.Map<IEnumerable<EnrollmentReadDto>>(enrollments);

        return new PagedResponseDto<EnrollmentReadDto>
        {
            Items = enrollmentReadDtos,
            TotalCount = totalCount,
            PageSize = searchDto.PageSize,
            PageNumber = searchDto.PageNumber
        };
    }

    public async Task<Result<EnrollmentReadDto>> CreateEnrollmentAsync(
        EnrollmentCreateDto createDto,
        CancellationToken ct = default)
    {
        if (await _enrollmentRepo.Exists(createDto.CourseId, createDto.UserId, ct))
        {
            _logger.LogWarning("Enrollment already exists for CourseId: {CourseId}, UserId: {UserId}.", createDto.CourseId, createDto.UserId);
            return Result<EnrollmentReadDto>.Failure(Error.EnrollmentAlreadyExists);
        }

        var enrollment = _mapper.Map<Enrollment>(createDto);

        await _enrollmentRepo.AddAsync(enrollment, ct);
        await _enrollmentRepo.SaveChangesAsync(ct);

        var enrollmentReadDto = _mapper.Map<EnrollmentReadDto>(enrollment);
        return Result<EnrollmentReadDto>.Success(enrollmentReadDto);
    }

    public async Task<Result<bool>> DeleteEnrollmentAsync(
        int id,
        CancellationToken ct = default)
    {
        Enrollment? enrollment = await _enrollmentRepo.GetByIdAsync(id, ct);

        if (enrollment is null)
        {
            _logger.LogInformation("Enrollment {EnrollmentId} not found for deletion.", id);
            return Result<bool>.Failure(Error.EnrollmentNotFound);
        }

        _enrollmentRepo.Remove(enrollment);
        await _enrollmentRepo.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
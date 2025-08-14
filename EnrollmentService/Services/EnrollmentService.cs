using AutoMapper;
using Common;
using Common.Errors;
using EnrollmentService.Data;
using EnrollmentService.Data.Queries.Implementations;
using EnrollmentService.Dtos;
using EnrollmentService.Messaging.Publishers;
using EnrollmentService.Models;
using EnrollmentService.Options;
using Microsoft.Extensions.Options;

namespace EnrollmentService.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IMapper _mapper;
    private readonly PaginationOptions _paginationOptions;
    private readonly IEnrollmentEventPublisher _publisher;

    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper,
        IOptions<PaginationOptions> paginationOptions,
        IEnrollmentEventPublisher publisher,
        ILogger<EnrollmentService> logger)
    {
        _enrollmentRepo = enrollmentRepository;
        _mapper = mapper;
        _paginationOptions = paginationOptions.Value;
        _publisher = publisher;
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
            return Result<EnrollmentReadDto>.Failure(EnrollmentErrors.EnrollmentNotFound);
        }

        var enrollmentReadDto = _mapper.Map<EnrollmentReadDto>(enrollment);
        
        return Result<EnrollmentReadDto>.Success(enrollmentReadDto);
    }

    public async Task<PagedResponseDto<EnrollmentReadDto>> SearchEnrollmentsAsync(
        EnrollmentSearchDto searchDto,
        CancellationToken ct = default)
    {
        EnrollmentQuery query = EnrollmentQuery.FromDto(searchDto, _paginationOptions);

        (IEnumerable<Enrollment> enrollments, int totalCount) =
            await _enrollmentRepo.SearchEnrollmentsAsync(query, ct);

        var enrollmentReadDtos = _mapper.Map<IEnumerable<EnrollmentReadDto>>(enrollments);

        return new PagedResponseDto<EnrollmentReadDto>
        {
            Items = enrollmentReadDtos,
            TotalCount = totalCount,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber
        };
    }

    public async Task<Result<EnrollmentReadDto>> CreateEnrollmentAsync(
        EnrollmentCreateDto createDto,
        CancellationToken ct = default)
    {
        if (await _enrollmentRepo.ExistsAsync(createDto.CourseId, createDto.UserId, ct))
        {
            _logger.LogWarning("Enrollment already exists for CourseId: {CourseId}, UserId: {UserId}.", createDto.CourseId, createDto.UserId);
            return Result<EnrollmentReadDto>.Failure(EnrollmentErrors.EnrollmentAlreadyExists);
        }

        var enrollment = _mapper.Map<Enrollment>(createDto);

        await _enrollmentRepo.AddAsync(enrollment, ct);
        await _enrollmentRepo.SaveChangesAsync(ct);

        await _publisher.PublishEnrollmentCreatedAsync(
            enrollment.Id, enrollment.UserId, enrollment.CourseId, correlationId: Guid.NewGuid());
         
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
            return Result<bool>.Failure(EnrollmentErrors.EnrollmentNotFound);
        }

        _enrollmentRepo.Remove(enrollment);
        await _enrollmentRepo.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
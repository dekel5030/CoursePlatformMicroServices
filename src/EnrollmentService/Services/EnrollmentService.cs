using AutoMapper;
using Common;
using Common.Errors;
using Common.Messaging.EventEnvelope;
using Enrollments.Contracts.Events;
using EnrollmentService.Data;
using EnrollmentService.Data.Queries.Implementations;
using EnrollmentService.Data.Repositories.Interfaces;
using EnrollmentService.Dtos;
using EnrollmentService.Messaging.Publishers;
using EnrollmentService.Models;
using EnrollmentService.Options;
using Microsoft.Extensions.Options;

namespace EnrollmentService.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IUserRepository _userRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly IMapper _mapper;
    private readonly PaginationOptions _paginationOptions;
    private readonly IEnrollmentEventPublisher _publisher;
    private readonly ILogger<EnrollmentService> _logger;
    private readonly string _producerName = "EnrollmentService";

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IMapper mapper,
        IOptions<PaginationOptions> paginationOptions,
        IEnrollmentEventPublisher publisher,
        ILogger<EnrollmentService> logger)
    {
        _enrollmentRepo = enrollmentRepository;
        _userRepo = userRepository;
        _courseRepo = courseRepository;
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

        if (await _userRepo.UserExistsAsync(createDto.UserId, ct) == false)
        {
            _logger.LogWarning("User {UserId} does not exist.", createDto.UserId);
            return Result<EnrollmentReadDto>.Failure(Error.UserNotFound);
        }

        if (await _courseRepo.CourseExistsAsync(createDto.CourseId, ct) == false)
        {
            _logger.LogWarning("Course {CourseId} does not exist.", createDto.CourseId);
            return Result<EnrollmentReadDto>.Failure(CourseErrors.CourseNotFound);
        }

        var enrollment = _mapper.Map<Enrollment>(createDto);

        await _enrollmentRepo.AddAsync(enrollment, ct);
        await _enrollmentRepo.SaveChangesAsync(ct);

        await PublishEnrollmentUpsertedAsync(enrollment, isActive: true, ct: ct);

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

        await PublishEnrollmentUpsertedAsync(enrollment, isActive: false, ct: ct);

        return Result<bool>.Success(true);
    }

    private Task PublishEnrollmentUpsertedAsync(
        Enrollment enrollment,
        bool isActive,
        string? eventId = null,
        string? correlationId = null,
        CancellationToken ct = default)
    {
        EnrollmentUpsertedV1 payload = new EnrollmentUpsertedV1
        (
            enrollment.Id,
            enrollment.UserId,
            enrollment.CourseId,
            isActive
        );

        var envelope = EventEnvelope<EnrollmentUpsertedV1>.Create(
            producer: _producerName,
            aggregateId: enrollment.Id.ToString(),
            aggregateVersion: enrollment.AggregateVersion,
            payload: payload,
            contractVersion: EnrollmentUpsertedV1.Version.ToString(),
            eventId: eventId,
            correlationId: correlationId
        );

        return _publisher.PublishEnrollmentUpsertedAsync(envelope, ct);
    }
}
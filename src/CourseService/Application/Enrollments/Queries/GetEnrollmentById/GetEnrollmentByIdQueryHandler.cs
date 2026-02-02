using Courses.Application.Enrollments.Dtos;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Queries.GetEnrollmentById;

internal sealed class GetEnrollmentByIdQueryHandler : IQueryHandler<GetEnrollmentByIdQuery, EnrollmentDto>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetEnrollmentByIdQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Result<EnrollmentDto>> Handle(
        GetEnrollmentByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Enrollment? enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (enrollment is null)
        {
            return Result.Failure<EnrollmentDto>(EnrollmentErrors.NotFound);
        }

        var dto = new EnrollmentDto(
            enrollment.Id.Value,
            enrollment.CourseId.Value,
            enrollment.StudentId.Value,
            enrollment.EnrolledAt,
            enrollment.ExpiresAt,
            enrollment.Status.ToString(),
            enrollment.CompletedAt);

        return Result.Success(dto);
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Courses.Domain.Enrollments.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.CreateEnrollment;

internal sealed class CreateEnrollmentCommandHandler : ICommandHandler<CreateEnrollmentCommand, CreateEnrollmentResponse>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEnrollmentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateEnrollmentResponse>> Handle(
        CreateEnrollmentCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CreateEnrollmentResponse>(CourseErrors.NotFound);
        }

        DateTimeOffset enrolledAt = request.EnrolledAt ?? DateTimeOffset.UtcNow;
        DateTimeOffset expiresAt = request.ExpiresAt ?? enrolledAt.AddYears(1);

        Result<Enrollment> enrollmentResult = Enrollment.Create(
            request.CourseId,
            request.StudentId,
            enrolledAt,
            expiresAt);

        if (enrollmentResult.IsFailure)
        {
            return Result.Failure<CreateEnrollmentResponse>(enrollmentResult.Error);
        }

        Enrollment enrollment = enrollmentResult.Value;

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateEnrollmentResponse(enrollment.Id.Value));
    }
}

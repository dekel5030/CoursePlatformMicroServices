using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.UpdateEnrollment;

internal sealed class UpdateEnrollmentCommandHandler : ICommandHandler<UpdateEnrollmentCommand>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEnrollmentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateEnrollmentCommand request,
        CancellationToken cancellationToken = default)
    {
        Enrollment? enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (enrollment is null)
        {
            return Result.Failure(EnrollmentErrors.NotFound);
        }

        if (request.Revoke == true)
        {
            enrollment.Revoke();
        }

        if (request.ExpiresAt.HasValue)
        {
            Result setExpiryResult = enrollment.SetExpiry(request.ExpiresAt.Value);

            if (setExpiryResult.IsFailure)
            {
                return setExpiryResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.DeleteEnrollment;

internal sealed class DeleteEnrollmentCommandHandler : ICommandHandler<DeleteEnrollmentCommand>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEnrollmentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteEnrollmentCommand request,
        CancellationToken cancellationToken = default)
    {
        Enrollment? enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (enrollment is null)
        {
            return Result.Failure(EnrollmentErrors.NotFound);
        }

        _enrollmentRepository.Remove(enrollment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

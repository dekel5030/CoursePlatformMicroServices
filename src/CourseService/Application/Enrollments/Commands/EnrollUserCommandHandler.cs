using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Enrollments;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands;

internal sealed class EnrollUserCommandHandler : ICommandHandler<EnrollUserCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly EnrollmentManager _enrollmentManager;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollUserCommandHandler(
        ICourseRepository courseRepository, 
        IEnrollmentRepository enrollmentRepository, 
        EnrollmentManager enrollmentManager,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _enrollmentManager = enrollmentManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        EnrollUserCommand request, 
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        
        if (course == null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        Result<Enrollment> result = _enrollmentManager.EnrollUser(course, request.UserId, request.ValidFor);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _enrollmentRepository.AddAsync(result.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

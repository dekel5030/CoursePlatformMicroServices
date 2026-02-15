using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PublishCourse;

public sealed class PublishCourseCommandHandler : ICommandHandler<PublishCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public PublishCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(
        PublishCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        if (_userContext.Id is null)
        {
            return Result.Failure(CourseErrors.Unauthorized);
        }

        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        if (course.InstructorId.Value != _userContext.Id.Value)
        {
            return Result.Failure(CourseErrors.Unauthorized);
        }

        Result result = course.Publish();

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

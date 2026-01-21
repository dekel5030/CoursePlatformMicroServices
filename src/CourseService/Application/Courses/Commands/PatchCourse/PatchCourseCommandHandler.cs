using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PatchCourse;

internal sealed class PatchCourseCommandHandler : ICommandHandler<PatchCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchCourseCommandHandler(
        ICourseRepository courseRepository, 
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PatchCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        course.UpdateDetails(request.Title, request.Description, request.Price);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

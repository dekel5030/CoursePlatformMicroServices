using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

internal sealed class PatchLessonCommandHandler : ICommandHandler<PatchLessonCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public PatchLessonCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfwORK,
        TimeProvider timeProvider)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfwORK;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(
        PatchLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        Result updateResult = course.UpdateLesson(
            request.LessonId,
            request.Title,
            request.Description,
            request.Access,
            _timeProvider);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

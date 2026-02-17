using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

internal sealed class PatchLessonCommandHandler : ICommandHandler<PatchLessonCommand>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchLessonCommandHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PatchLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        Result updateResult = lesson.UpdateMetadata(
            request.Title ?? lesson.Title,
            request.Description ?? lesson.Description,
            lesson.Slug);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        if (request.Access.HasValue)
        {
            Result accessResult = lesson.ChangeAccess(request.Access.Value);
            if (accessResult.IsFailure)
            {
                return accessResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

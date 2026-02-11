using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.MoveLesson;

internal sealed class MoveLessonCommandHandler : ICommandHandler<MoveLessonCommand>
{
    private readonly LessonManagementService _lessonManagement;
    private readonly IUnitOfWork _unitOfWork;

    public MoveLessonCommandHandler(
        LessonManagementService lessonManagement,
        IUnitOfWork unitOfWork)
    {
        _lessonManagement = lessonManagement;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        MoveLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Result result = await _lessonManagement.MoveLessonAsync(
            request.LessonId,
            request.TargetModuleId,
            request.TargetIndex,
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

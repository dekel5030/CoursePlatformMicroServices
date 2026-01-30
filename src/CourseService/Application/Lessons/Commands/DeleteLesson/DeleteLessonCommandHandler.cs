using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public class DeleteLessonCommandHandler : ICommandHandler<DeleteLessonCommand>
{
    private readonly LessonManagementService _lessonManagement;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLessonCommandHandler(LessonManagementService lessonManagement, IUnitOfWork unitOfWork)
    {
        _lessonManagement = lessonManagement;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Result result = await _lessonManagement.RemoveLessonAsync(request.LessonId, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.ReorderLessons;

internal sealed class ReorderLessonsCommandHandler : ICommandHandler<ReorderLessonsCommand>
{
    private readonly LessonManagementService _lessonManagement;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderLessonsCommandHandler(
        LessonManagementService lessonManagement,
        IUnitOfWork unitOfWork)
    {
        _lessonManagement = lessonManagement;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReorderLessonsCommand request,
        CancellationToken cancellationToken = default)
    {
        Result result = await _lessonManagement.ReorderLessonsAsync(
            request.ModuleId,
            request.LessonIds,
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

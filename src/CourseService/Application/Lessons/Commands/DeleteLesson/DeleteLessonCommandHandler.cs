using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Module;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public class DeleteLessonCommandHandler : ICommandHandler<DeleteLessonCommand>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLessonCommandHandler(IModuleRepository moduleRepository, IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Module? module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        if (module is null)
        {
            return Result.Failure(Error.NotFound("Module.NotFound", "The specified module was not found."));
        }

        Result deletionResult = module.RemoveLesson(request.LessonId);
        if (deletionResult.IsFailure)
        {
            return Result.Failure(deletionResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

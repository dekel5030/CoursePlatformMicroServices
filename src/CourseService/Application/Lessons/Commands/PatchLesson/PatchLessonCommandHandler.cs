using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

internal sealed class PatchLessonCommandHandler : ICommandHandler<PatchLessonCommand>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchLessonCommandHandler(
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PatchLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Module? module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);

        if (module is null)
        {
            return Result.Failure(ModuleErrors.NotFound);
        }

        Result updateResult = module.UpdateLesson(
            request.LessonId,
            request.Title,
            request.Description,
            request.Access,
            null);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

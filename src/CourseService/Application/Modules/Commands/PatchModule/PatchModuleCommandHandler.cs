using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Errors;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.PatchModule;

internal sealed class PatchModuleCommandHandler : ICommandHandler<PatchModuleCommand>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchModuleCommandHandler(
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PatchModuleCommand request,
        CancellationToken cancellationToken = default)
    {
        Module? module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);

        if (module is null)
        {
            return Result.Failure(ModuleErrors.NotFound);
        }

        if (request.Title is not null)
        {
            Result updateResult = module.UpdateTitle(request.Title);
            if (updateResult.IsFailure)
            {
                return updateResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

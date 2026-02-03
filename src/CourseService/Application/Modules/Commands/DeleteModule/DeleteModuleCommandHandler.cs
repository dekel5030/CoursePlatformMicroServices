using Courses.Application.Abstractions.Data;
using Courses.Domain.Modules;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.DeleteModule;

internal sealed class DeleteModuleCommandHandler : ICommandHandler<DeleteModuleCommand>
{
    private readonly ModuleManagementService _moduleManagement;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteModuleCommandHandler(
        ModuleManagementService moduleManagement,
        IUnitOfWork unitOfWork)
    {
        _moduleManagement = moduleManagement;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteModuleCommand request,
        CancellationToken cancellationToken = default)
    {
        Result result = await _moduleManagement.RemoveModuleAsync(request.ModuleId, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

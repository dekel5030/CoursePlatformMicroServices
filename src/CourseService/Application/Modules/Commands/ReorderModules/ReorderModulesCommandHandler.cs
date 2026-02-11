using Courses.Application.Abstractions.Data;
using Courses.Domain.Modules;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.ReorderModules;

internal sealed class ReorderModulesCommandHandler : ICommandHandler<ReorderModulesCommand>
{
    private readonly ModuleManagementService _moduleManagement;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderModulesCommandHandler(
        ModuleManagementService moduleManagement,
        IUnitOfWork unitOfWork)
    {
        _moduleManagement = moduleManagement;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReorderModulesCommand request,
        CancellationToken cancellationToken = default)
    {
        Result result = await _moduleManagement.ReorderModulesAsync(
            request.CourseId,
            request.ModuleIds,
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

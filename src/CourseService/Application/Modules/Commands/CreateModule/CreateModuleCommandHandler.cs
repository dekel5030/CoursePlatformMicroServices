using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Modules;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.CreateModule;

internal sealed class CreateModuleCommandHandler : ICommandHandler<CreateModuleCommand, CreateModuleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ModuleManagementService _moduleManagement;

    public CreateModuleCommandHandler(
        ModuleManagementService moduleManagement,
        IUnitOfWork unitOfWork)
    {
        _moduleManagement = moduleManagement;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateModuleResponse>> Handle(
        CreateModuleCommand request,
        CancellationToken cancellationToken = default)
    {
        Result<Module> result = await _moduleManagement.CreateModuleAsync(
            request.CourseId,
            request.Title ?? Title.Empty,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<CreateModuleResponse>(result.Error);
        }

        Module module = result.Value;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateModuleResponse(module.Id.Value, module.CourseId.Value, module.Title.Value));
    }
}

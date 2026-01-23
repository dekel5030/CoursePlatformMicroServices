using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Module;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.CreateModule;

internal sealed class CreateModuleCommandHandler : ICommandHandler<CreateModuleCommand, CreateModuleResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateModuleCommandHandler(
        ICourseRepository courseRepository,
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateModuleResponse>> Handle(
        CreateModuleCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CreateModuleResponse>(CourseErrors.NotFound);
        }

        IReadOnlyList<Module> existingModules = await _moduleRepository
            .GetAllByCourseIdAsync(request.CourseId, cancellationToken);

        int nextIndex = existingModules.Count;

        Result<Module> moduleResult = Module.Create(request.CourseId, nextIndex, request.Title);

        if (moduleResult.IsFailure)
        {
            return Result.Failure<CreateModuleResponse>(moduleResult.Error);
        }

        Module module = moduleResult.Value;

        await _moduleRepository.AddAsync(module, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateModuleResponse(module.Id, module.CourseId, module.Title));
    }
}

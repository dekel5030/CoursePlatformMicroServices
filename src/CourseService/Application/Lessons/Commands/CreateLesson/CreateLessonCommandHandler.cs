using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, CreateLessonResponse>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly LessonManagementService _lessonManagementService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLessonCommandHandler(
        IModuleRepository moduleRepository,
        LessonManagementService lessonManagementService,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _lessonManagementService = lessonManagementService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateLessonResponse>> Handle(
        CreateLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Module? module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);

        if (module is null)
        {
            return Result.Failure<CreateLessonResponse>(Error.NotFound("Module.NotFound", "The specified module was not found."));
        }

        Result<Lesson> result = await _lessonManagementService.CreateLessonAsync(
            module.CourseId,
            request.ModuleId,
            request.Title ?? Title.Empty,
            request.Description ?? Description.Empty,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<CreateLessonResponse>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateLessonResponse(module.CourseId.Value, module.Id.Value));
    }
}

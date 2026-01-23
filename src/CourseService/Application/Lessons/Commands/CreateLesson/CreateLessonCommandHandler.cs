using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Module;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, CreateLessonResponse>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLessonCommandHandler(
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
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

        Result result = module.AddLesson(request.Title, request.Description);

        if (result.IsFailure)
        {
            return Result.Failure<CreateLessonResponse>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);


        return Result.Success(new CreateLessonResponse(module.CourseId, module.Id));
    }
}

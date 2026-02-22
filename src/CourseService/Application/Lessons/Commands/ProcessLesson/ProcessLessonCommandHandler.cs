using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.ProcessLesson;

internal sealed class ProcessLessonCommandHandler : ICommandHandler<ProcessLessonCommand>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessLessonCommandHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ProcessLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return LessonErrors.NotFound;
        }
        
        lesson.SubmitToPostProduction(request.Message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

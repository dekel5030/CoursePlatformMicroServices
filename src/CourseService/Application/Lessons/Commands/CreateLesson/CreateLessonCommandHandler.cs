using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Lessons;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, CreateLessonResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateLessonCommandHandler(
        ICourseRepository courseRepository,
        TimeProvider timeProvider,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateLessonResponse>> Handle(
        CreateLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CreateLessonResponse>(CourseErrors.NotFound);
        }

        Result<Lesson> result = course.AddLesson(request.Title, request.Description, _timeProvider);

        if (result.IsFailure)
        {
            return Result.Failure<CreateLessonResponse>(result.Error);
        }

        Lesson lesson = result.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateLessonResponse(
            lesson.CourseId,
            lesson.Id,
            lesson.Title));
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public class DeleteLessonCommandHandler : ICommandHandler<DeleteLessonCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLessonCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteLessonCommand request, 
        CancellationToken cancellationToken = default)
    {
        LessonId lessonId = new LessonId(request.LessonId);
        CourseId courseId = new CourseId(request.CourseId);

        Course? course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        Result deletionResult = course.DeleteLesson(lessonId);
        if (deletionResult.IsFailure)
        {
            return Result.Failure(deletionResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

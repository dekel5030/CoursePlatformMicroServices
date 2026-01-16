using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions.Abstract;
using Courses.Application.Lessons.Extensions;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, LessonSummaryDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IStorageUrlResolver _urlResolver;
#pragma warning restore S4487 // Unread "private" fields should be removed
    private readonly ICourseActionProvider _actionProvider;

    public CreateLessonCommandHandler(
        ICourseRepository courseRepository,
        TimeProvider timeProvider,
        IStorageUrlResolver urlResolver,
        IUnitOfWork unitOfWork,
        ICourseActionProvider actionProvider)
    {
        _courseRepository = courseRepository;
        _timeProvider = timeProvider;
        _urlResolver = urlResolver;
        _unitOfWork = unitOfWork;
        _actionProvider = actionProvider;
    }

    public async Task<Result<LessonSummaryDto>> Handle(
        CreateLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonSummaryDto>(CourseErrors.NotFound);
        }

        Result<Lesson> result = course.AddLesson(request.Title, request.Description, _timeProvider);

        if (result.IsFailure)
        {
            return Result.Failure<LessonSummaryDto>(result.Error);
        }

        Lesson lesson = result.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        LessonSummaryDto response = new(
            lesson.CourseId,
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Index,
            lesson.Duration,
            lesson.Access == LessonAccess.Public,
            lesson.ThumbnailImageUrl,
            _actionProvider.GetAllowedActions(course, lesson));

        return Result.Success(response);
    }
}

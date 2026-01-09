using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Extensions;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, LessonDetailsDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IStorageUrlResolver _urlResolver;

    public CreateLessonCommandHandler(
        ICourseRepository courseRepository,
        TimeProvider timeProvider,
        IStorageUrlResolver urlResolver,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _timeProvider = timeProvider;
        _urlResolver = urlResolver;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        CreateLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.CourseId);

        Course? course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonDetailsDto>(CourseErrors.NotFound);
        }

        Title? lessonTitle = request.Title is null ? null : new Title(request.Title);
        Description? lessonDescription = request.Description is null ? null : new Description(request.Description);

        Result<Lesson> result = course.AddLesson(lessonTitle, lessonDescription, _timeProvider);

        if (result.IsFailure)
        {
            return Result.Failure<LessonDetailsDto>(result.Error);
        }

        Lesson lesson = result.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        LessonDetailsDto response = await lesson.ToDetailsDtoAsync(_urlResolver, cancellationToken);

        return Result.Success(response);
    }
}

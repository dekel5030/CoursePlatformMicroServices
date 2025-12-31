using Courses.Application.Abstractions;
using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, LessonDetailsDto>
{
    private readonly IWriteDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IUrlResolver _urlResolver;

    public CreateLessonCommandHandler(
        IWriteDbContext dbContext,
        TimeProvider timeProvider,
        IUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _urlResolver = urlResolver;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        CreateLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        CreateLessonDto dto = request.Dto;

        var courseId = new CourseId(dto.CourseId);

        var course = await _dbContext.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonDetailsDto>(CourseErrors.NotFound);
        }

        Title? lessonTitle = dto.Title is null ? null : new Title(dto.Title);
        Description? lessonDescription = dto.Description is null ? null : new Description(dto.Description);

        var result = course.AddLesson(lessonTitle, lessonDescription, _timeProvider);

        if (result.IsFailure)
        {
            return Result.Failure<LessonDetailsDto>(result.Error);
        }

        var lesson = result.Value;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new LessonDetailsDto(
            lesson.Id.Value,
            lesson.Title.Value,
            lesson.Description.Value,
            lesson.Index,
            lesson.Duration,
            lesson.Access == LessonAccess.Public,
            _urlResolver.Resolve(lesson.ThumbnailImageUrl?.Path ?? string.Empty),
            _urlResolver.Resolve(lesson.VideoUrl?.Path ?? string.Empty)
        );

        return Result.Success(response);
    }
}

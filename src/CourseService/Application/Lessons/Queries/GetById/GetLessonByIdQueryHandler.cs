using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

internal sealed class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsPageDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ILinkBuilderService _linkBuilder;

    public GetLessonByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        ILinkBuilderService linkBuilder)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<LessonDetailsPageDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(lesson => lesson.Id == request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        string? thumbnailUrl = lesson.ThumbnailImageUrl != null
            ? _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value
            : null;

        string? videoUrl = lesson.VideoUrl != null
            ? _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl.Path).Value
            : null;

        string? transcriptUrl = lesson.Transcript != null
            ? _urlResolver.Resolve(StorageCategory.Public, lesson.Transcript.Path).Value
            : null;

        var courseState = new CourseState(course.Id, course.InstructorId, course.Status);
        var moduleState = new ModuleState(lesson.ModuleId);
        var lessonState = new LessonState(lesson.Id, lesson.Access);
        var lessonContext = new LessonLinkContext(courseState, moduleState, lessonState, null);

        var pageDto = new LessonDetailsPageDto
        {
            LessonId = lesson.Id.Value,
            ModuleId = lesson.ModuleId.Value,
            CourseId = lesson.CourseId.Value,
            CourseName = course.Title.Value,
            Title = lesson.Title.Value,
            Description = lesson.Description.Value,
            Index = lesson.Index,
            Duration = lesson.Duration,
            ThumbnailUrl = thumbnailUrl,
            Access = lesson.Access,
            VideoUrl = videoUrl,
            TranscriptUrl = transcriptUrl,
            Links = _linkBuilder.BuildLinks(LinkResourceKey.Lesson, lessonContext)
        };

        return Result.Success(pageDto);
    }
}

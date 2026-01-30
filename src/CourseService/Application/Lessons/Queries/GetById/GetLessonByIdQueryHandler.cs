using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

internal sealed class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsPageDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ILessonLinkFactory _lessonLinkFactory;

    public GetLessonByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        ILessonLinkFactory lessonLinkFactory)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _lessonLinkFactory = lessonLinkFactory;
    }

    public async Task<Result<LessonDetailsPageDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        LessonReadModel? lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == request.LessonId.Value, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        CourseReadModel? course = await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        string? thumbnailUrl = lesson.ThumbnailUrl != null
            ? _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailUrl).Value
            : null;

        string? videoUrl = lesson.VideoUrl != null
            ? _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl).Value
            : null;

        string? transcriptUrl = lesson.TranscriptUrl != null
            ? _urlResolver.Resolve(StorageCategory.Public, lesson.TranscriptUrl).Value
            : null;

        var courseState = new CourseState(new CourseId(course.Id), new UserId(course.InstructorId), course.Status);
        var moduleState = new ModuleState(new ModuleId(lesson.ModuleId));
        var lessonState = new LessonState(new LessonId(lesson.Id), lesson.Access);

        var pageDto = new LessonDetailsPageDto
        {
            LessonId = lesson.Id,
            ModuleId = lesson.ModuleId,
            CourseId = lesson.CourseId,
            CourseName = course.Title,
            Title = lesson.Title,
            Description = lesson.Description,
            Index = lesson.Index,
            Duration = lesson.Duration,
            ThumbnailUrl = thumbnailUrl,
            Access = lesson.Access,
            VideoUrl = videoUrl,
            TranscriptUrl = transcriptUrl,
            Links = _lessonLinkFactory.CreateLinks(courseState, moduleState, lessonState)
        };

        return Result.Success(pageDto);
    }
}

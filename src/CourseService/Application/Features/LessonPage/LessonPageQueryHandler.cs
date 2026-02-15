using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.LessonPage;
using Courses.Application.Features.Shared;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.LessonPage;

internal sealed class LessonPageQueryHandler : IQueryHandler<LessonPageQuery, LessonPageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILinkProvider _linkProvider;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public LessonPageQueryHandler(
        IReadDbContext readDbContext,
        ILinkProvider linkProvider,
        IStorageUrlResolver storageUrlResolver)
    {
        _readDbContext = readDbContext;
        _linkProvider = linkProvider;
        _storageUrlResolver = storageUrlResolver;
    }

    public async Task<Result<LessonPageDto>> Handle(
        LessonPageQuery request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _readDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        LessonPageData data = MapData(lesson, course.Title.Value);
        LessonPageLinks links = new(
            Self: _linkProvider.GetLessonPageLink(lesson.Id.Value),
            Course: _linkProvider.GetCoursePageLink(lesson.CourseId.Value));

        return Result.Success(new LessonPageDto(Data: data, Links: links));
    }

    private LessonPageData MapData(Lesson lesson, string courseName)
    {
        return new LessonPageData(
            LessonId: lesson.Id.Value,
            ModuleId: lesson.ModuleId.Value,
            CourseId: lesson.CourseId.Value,
            CourseName: courseName,
            Title: lesson.Title.Value,
            Description: lesson.Description.Value,
            Index: lesson.Index,
            Duration: lesson.Duration,
            Access: lesson.Access,
            ThumbnailUrl: _storageUrlResolver.ResolvePublicUrl(lesson.ThumbnailImageUrl?.Path),
            VideoUrl: _storageUrlResolver.ResolvePublicUrl(lesson.VideoUrl?.Path),
            TranscriptUrl: _storageUrlResolver.ResolvePublicUrl(lesson.Transcript?.Path));
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.LessonPage;
using Courses.Application.Features.Shared;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
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
        (Guid? nextLessonId, Guid? previousLessonId) = await ResolveAdjacentLessonIdsAsync(
            lesson.CourseId,
            lesson.Id.Value,
            cancellationToken);

        LessonPageLinks links = new(
            Self: _linkProvider.GetLessonPageLink(lesson.Id.Value),
            Course: _linkProvider.GetCoursePageLink(lesson.CourseId.Value),
            NextLesson: nextLessonId is { } nid ? _linkProvider.GetLessonPageLink(nid) : null,
            PreviousLesson: previousLessonId is { } pid ? _linkProvider.GetLessonPageLink(pid) : null,
            MarkAsComplete: _linkProvider.GetPlaceholderLink("POST"),
            UnmarkAsComplete: _linkProvider.GetPlaceholderLink("DELETE"),
            Manage: _linkProvider.GetManagedLessonPageLink(lesson.Id.Value));

        return Result.Success(new LessonPageDto(Data: data, Links: links));
    }

    private async Task<(Guid? NextLessonId, Guid? PreviousLessonId)> ResolveAdjacentLessonIdsAsync(
        CourseId courseId,
        Guid currentLessonId,
        CancellationToken cancellationToken)
    {
        List<Guid> moduleOrder = await _readDbContext.Modules
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Index)
            .Select(m => m.Id.Value)
            .ToListAsync(cancellationToken);

        var lessonOrder = await _readDbContext.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => new { l.Id, l.ModuleId, l.Index })
            .ToListAsync(cancellationToken);

        var ordered = lessonOrder
            .OrderBy(l => moduleOrder.IndexOf(l.ModuleId.Value))
            .ThenBy(l => l.Index)
            .Select(l => l.Id.Value)
            .ToList();

        int idx = ordered.IndexOf(currentLessonId);
        if (idx < 0)
        {
            return (null, null);
        }

        Guid? next = idx + 1 < ordered.Count ? ordered[idx + 1] : null;
        Guid? prev = idx > 0 ? ordered[idx - 1] : null;
        return (next, prev);
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

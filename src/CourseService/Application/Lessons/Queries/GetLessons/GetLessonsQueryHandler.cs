using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetLessons;

internal sealed class GetLessonsQueryHandler : IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonDto>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetLessonsQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<IReadOnlyList<LessonDto>>> Handle(
    GetLessonsQuery request,
    CancellationToken cancellationToken = default)
    {
        IQueryable<Lesson> query = _readDbContext.Lessons;
        query = ApplyFilters(query, request.Filter);

        List<Lesson> lessons = await query
            .OrderBy(l => l.ModuleId).ThenBy(l => l.Index)
            .ToListAsync(cancellationToken);

        if (lessons.Count == 0)
        {
            return Result.Success<IReadOnlyList<LessonDto>>([]);
        }

        Dictionary<CourseId, Course> coursesById = await GetRequiredCoursesAsync(lessons, cancellationToken);

        var response = lessons.Select(lesson => MapToDto(
            lesson,
            coursesById[lesson.CourseId])).ToList();

        return Result.Success<IReadOnlyList<LessonDto>>(response);
    }

    private async Task<Dictionary<CourseId, Course>> GetRequiredCoursesAsync(
        List<Lesson> lessons,
        CancellationToken cancellationToken)
    {
        var courseIds = lessons.Select(l => l.CourseId).Distinct().ToList();

        List<Course> courses = await _readDbContext.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return courses.ToDictionary(c => c.Id);
    }

    private static IQueryable<Lesson> ApplyFilters(IQueryable<Lesson> query, LessonFilter filter)
    {
        if (filter.CourseId is { } courseId)
        {
            query = query.Where(l => l.CourseId == courseId);
        }

        if (filter.Id is { } id)
        {
            query = query.Where(l => l.Id == id);
        }

        if (filter.Ids is { } ids)
        {
            query = query.Where(l => ids.Select(i => new LessonId(i)).Contains(l.Id));
        }

        return query;
    }

    private LessonDto MapToDto(
        Lesson lesson,
        Course course)
    {
        var dto = new LessonDto
        {
            Id = lesson.Id.Value,
            Title = lesson.Title.Value,
            Index = lesson.Index,
            Duration = lesson.Duration,
            Access = lesson.Access,
            ThumbnailUrl = ResolveUrl(lesson.ThumbnailImageUrl?.Path),
            ModuleId = lesson.ModuleId.Value,
            CourseId = lesson.CourseId.Value,
            CourseName = course.Title.Value,
            Description = lesson.Description.Value,
            VideoUrl = ResolveUrl(lesson.VideoUrl?.Path),
            TranscriptUrl = ResolveUrl(lesson.Transcript?.Path),
            Links = []
        };

        return dto;
    }

    private string? ResolveUrl(string? path)
    {
        return path is not null ? _urlResolver.Resolve(StorageCategory.Public, path).Value : null;
    }
}

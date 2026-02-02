using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
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
    private readonly ILinkBuilderService _linkBuilder;

    public GetLessonsQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver,
        ILinkBuilderService linkBuilder)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<IReadOnlyList<LessonDto>>> Handle(
        GetLessonsQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Lesson> query = _readDbContext.Lessons.AsNoTracking();

        if (request.Filter.CourseId is { } courseId)
        {
            query = query.Where(l => l.CourseId == courseId);
        }

        if (request.Filter.Id is { } singleId)
        {
            query = query.Where(l => l.Id == singleId);
        }
        else if (request.Filter.Ids is { } idsEnumerable)
        {
            var ids = idsEnumerable.Distinct().Select(id => new LessonId(id)).ToList();
            if (ids.Count > 0)
            {
                query = query.Where(l => ids.Contains(l.Id));
            }
        }

        List<Lesson> lessons = await query
            .OrderBy(l => l.ModuleId)
            .ThenBy(l => l.Index)
            .ToListAsync(cancellationToken);

        if (lessons.Count == 0)
        {
            return Result.Success<IReadOnlyList<LessonDto>>([]);
        }

        var courseIds = lessons.Select(l => l.CourseId).Distinct().ToList();
        List<Course> courses = await _readDbContext.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
        var coursesById = courses.ToDictionary(c => c.Id);

        var moduleContextsByKey = new Dictionary<(CourseId CourseId, ModuleId ModuleId), ModuleContext>();
        foreach (Course course in courses)
        {
            var courseContext = new CourseContext(course.Id, course.InstructorId, course.Status);
            foreach (ModuleId moduleId in lessons.Where(l => l.CourseId == course.Id).Select(l => l.ModuleId).Distinct())
            {
                moduleContextsByKey[(course.Id, moduleId)] = new ModuleContext(courseContext, moduleId);
            }
        }

        bool includeDetails = request.Filter.IncludeDetails;
        var result = lessons.Select(lesson =>
        {
            Course? course = coursesById.GetValueOrDefault(lesson.CourseId);
            ModuleContext? moduleContext = course is not null
                ? moduleContextsByKey.GetValueOrDefault<(CourseId, ModuleId), ModuleContext>((lesson.CourseId, lesson.ModuleId))
                : null;

            string? thumbnailUrl = lesson.ThumbnailImageUrl is not null
                ? _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value
                : null;

            string? videoUrl = null;
            string? transcriptUrl = null;
            string? courseName = null;
            string? description = null;

            if (includeDetails)
            {
                videoUrl = lesson.VideoUrl is not null
                    ? _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl.Path).Value
                    : null;
                transcriptUrl = lesson.Transcript is not null
                    ? _urlResolver.Resolve(StorageCategory.Public, lesson.Transcript.Path).Value
                    : null;
                courseName = course?.Title.Value;
                description = lesson.Description.Value;
            }

            List<LinkDto> links = moduleContext is not null
                ? _linkBuilder.BuildLinks(LinkResourceKey.Lesson, new LessonContext(
                    moduleContext,
                    lesson.Id,
                    lesson.Access,
                    HasEnrollment: false)).ToList()
                : [];

            return new LessonDto
            {
                Id = lesson.Id.Value,
                Title = lesson.Title.Value,
                Index = lesson.Index,
                Duration = lesson.Duration,
                ThumbnailUrl = thumbnailUrl,
                Access = lesson.Access,
                Links = links,
                ModuleId = includeDetails ? lesson.ModuleId.Value : null,
                CourseId = includeDetails ? lesson.CourseId.Value : null,
                CourseName = courseName,
                Description = description,
                VideoUrl = videoUrl,
                TranscriptUrl = transcriptUrl
            };
        }).ToList();

        return Result.Success<IReadOnlyList<LessonDto>>(result);
    }
}

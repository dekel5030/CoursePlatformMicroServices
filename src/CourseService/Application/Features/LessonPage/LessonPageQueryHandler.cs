using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Services.LinkProvider;
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
    private readonly ILinkBuilderService _linkBuilderService;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public LessonPageQueryHandler(
        IReadDbContext readDbContext,
        ILinkBuilderService linkBuilderService,
        IStorageUrlResolver storageUrlResolver)
    {
        _readDbContext = readDbContext;
        _linkBuilderService = linkBuilderService;
        _storageUrlResolver = storageUrlResolver;
    }

    public async Task<Result<LessonPageDto>> Handle(
        LessonPageQuery request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _readDbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _readDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        CourseContext courseContext = new(
            course.Id,
            course.InstructorId,
            course.Status,
            IsManagementView: false);

        ModuleContext moduleContext = new(courseContext, lesson.ModuleId);
        LessonContext lessonContext = new(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);

        LessonPageDto dto = MapToDto(lesson, course.Title.Value, lessonContext);

        return Result.Success(dto);
    }

    private LessonPageDto MapToDto(Lesson lesson, string courseName, LessonContext lessonContext)
    {
        return new LessonPageDto
        {
            LessonId = lesson.Id.Value,
            ModuleId = lesson.ModuleId.Value,
            CourseId = lesson.CourseId.Value,
            CourseName = courseName,
            Title = lesson.Title.Value,
            Description = lesson.Description.Value,
            Index = lesson.Index,
            Duration = lesson.Duration,
            Access = lesson.Access,
            ThumbnailUrl = ResolveUrl(lesson.ThumbnailImageUrl?.Path),
            VideoUrl = ResolveUrl(lesson.VideoUrl?.Path),
            TranscriptUrl = ResolveUrl(lesson.Transcript?.Path),
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Lesson, lessonContext).ToList()
        };
    }

    private string? ResolveUrl(string? path)
    {
        return path is not null ? _storageUrlResolver.Resolve(StorageCategory.Public, path).Value : null;
    }
}

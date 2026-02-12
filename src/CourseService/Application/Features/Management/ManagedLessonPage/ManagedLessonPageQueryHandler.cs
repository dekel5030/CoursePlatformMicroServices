using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedLessonPage;

internal sealed class ManagedLessonPageQueryHandler
    : IQueryHandler<ManagedLessonPageQuery, LessonDetailsPageDto>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ILinkBuilderService _linkBuilderService;
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly IUserContext _userContext;

    public ManagedLessonPageQueryHandler(
        IWriteDbContext writeDbContext,
        ILinkBuilderService linkBuilderService,
        IStorageUrlResolver storageUrlResolver,
        IUserContext userContext)
    {
        _writeDbContext = writeDbContext;
        _linkBuilderService = linkBuilderService;
        _storageUrlResolver = storageUrlResolver;
        _userContext = userContext;
    }

    public async Task<Result<LessonDetailsPageDto>> Handle(
        ManagedLessonPageQuery request,
        CancellationToken cancellationToken = default)
    {
        if (_userContext.Id is null || !_userContext.IsAuthenticated)
        {
            return Result.Failure<LessonDetailsPageDto>(CourseErrors.Unauthorized);
        }

        var lessonId = new LessonId(request.LessonId);
        var instructorId = new UserId(_userContext.Id.Value);

        Lesson? lesson = await _writeDbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson == null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _writeDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course == null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        if (course.InstructorId != instructorId)
        {
            return Result.Failure<LessonDetailsPageDto>(CourseErrors.Unauthorized);
        }

        CourseContext courseContext = new(
            course.Id,
            course.InstructorId,
            course.Status,
            IsManagementView: true);

        ModuleContext moduleContext = new(courseContext, lesson.ModuleId);
        LessonContext lessonContext = new(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);

        LessonDetailsPageDto dto = MapToDto(lesson, course.Title.Value, lessonContext);

        return Result.Success(dto);
    }

    private LessonDetailsPageDto MapToDto(Lesson lesson, string courseName, LessonContext lessonContext)
    {
        return new LessonDetailsPageDto
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

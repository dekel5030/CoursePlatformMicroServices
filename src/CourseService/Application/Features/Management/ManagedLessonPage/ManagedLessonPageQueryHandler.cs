using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.Shared;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedLessonPage;

internal sealed class ManagedLessonPageQueryHandler
    : IQueryHandler<ManagedLessonPageQuery, ManagedLessonPageDto>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ILinkProvider _linkProvider;
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly CourseGovernancePolicy _policy;
    private readonly IUserContext _userContext;

    public ManagedLessonPageQueryHandler(
        IWriteDbContext writeDbContext,
        ILinkProvider linkProvider,
        IStorageUrlResolver storageUrlResolver,
        CourseGovernancePolicy policy,
        IUserContext userContext)
    {
        _writeDbContext = writeDbContext;
        _linkProvider = linkProvider;
        _storageUrlResolver = storageUrlResolver;
        _policy = policy;
        _userContext = userContext;
    }

    public async Task<Result<ManagedLessonPageDto>> Handle(
        ManagedLessonPageQuery request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _writeDbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson == null)
        {
            return Result.Failure<ManagedLessonPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _writeDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course == null)
        {
            return Result.Failure<ManagedLessonPageDto>(LessonErrors.NotFound);
        }

        Result<UserId> authResult = InstructorAuthorization.EnsureInstructorAuthorized(
            _userContext,
            course.InstructorId);

        if (authResult.IsFailure)
        {
            return Result.Failure<ManagedLessonPageDto>(CourseErrors.Unauthorized);
        }

        var courseContext = new CourseContext(
            course.Id,
            course.InstructorId,
            course.Status,
            IsManagementView: true);
        var moduleContext = new ModuleContext(courseContext, lesson.ModuleId);
        var lessonContext = new LessonContext(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);

        bool canEdit = _policy.CanEditLesson(lessonContext);
        Guid lid = lesson.Id.Value;
        Guid cid = lesson.CourseId.Value;

        ManagedLessonPageLinks links = new(
            Self: _linkProvider.GetLessonPageLink(lid),
            Course: _linkProvider.GetCoursePageLink(cid),
            PartialUpdate: canEdit ? _linkProvider.GetPatchLessonLink(lid) : null,
            UploadVideoUrl: canEdit ? _linkProvider.GetLessonVideoUploadUrlLink(lid) : null,
            AiGenerate: canEdit ? _linkProvider.GetGenerateLessonWithAiLink(lid) : null,
            Move: canEdit ? _linkProvider.GetMoveLessonLink(lid) : null);

        ManagedLessonPageData data = new(
            LessonId: lesson.Id.Value,
            ModuleId: lesson.ModuleId.Value,
            CourseId: lesson.CourseId.Value,
            CourseName: course.Title.Value,
            Title: lesson.Title.Value,
            Description: lesson.Description.Value,
            Index: lesson.Index,
            Duration: lesson.Duration,
            Access: lesson.Access,
            ThumbnailUrl: _storageUrlResolver.ResolvePublicUrl(lesson.ThumbnailImageUrl?.Path),
            VideoUrl: _storageUrlResolver.ResolvePublicUrl(lesson.VideoUrl?.Path),
            TranscriptUrl: _storageUrlResolver.ResolvePublicUrl(lesson.Transcript?.Path));

        return Result.Success(new ManagedLessonPageDto(Data: data, Links: links));
    }
}

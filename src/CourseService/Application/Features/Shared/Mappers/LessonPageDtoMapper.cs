using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.LessonPage;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons;

namespace Courses.Application.Features.Shared.Mappers;

internal sealed class LessonPageDtoMapper : ILessonPageDtoMapper
{
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly ILinkProvider _linkProvider;

    public LessonPageDtoMapper(
        IStorageUrlResolver storageUrlResolver,
        ILinkProvider linkProvider)
    {
        _storageUrlResolver = storageUrlResolver;
        _linkProvider = linkProvider;
    }

    public LessonPageDto Map(Lesson lesson, string courseName, LessonContext lessonContext)
    {
        var data = new LessonPageData(
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
        var links = new LessonPageLinks(
            Self: _linkProvider.GetLessonPageLink(lesson.Id.Value),
            Course: _linkProvider.GetCoursePageLink(lesson.CourseId.Value),
            NextLesson: null,
            PreviousLesson: null,
            MarkAsComplete: null,
            UnmarkAsComplete: null,
            Manage: _linkProvider.GetManagedLessonPageLink(lesson.Id.Value));
        return new LessonPageDto(Data: data, Links: links);
    }
}

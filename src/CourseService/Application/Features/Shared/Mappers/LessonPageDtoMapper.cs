using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.LessonPage;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons;

namespace Courses.Application.Features.Shared.Mappers;

internal sealed class LessonPageDtoMapper : ILessonPageDtoMapper
{
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly ILinkBuilderService _linkBuilderService;

    public LessonPageDtoMapper(
        IStorageUrlResolver storageUrlResolver,
        ILinkBuilderService linkBuilderService)
    {
        _storageUrlResolver = storageUrlResolver;
        _linkBuilderService = linkBuilderService;
    }

    public LessonPageDto Map(Lesson lesson, string courseName, LessonContext lessonContext)
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
            ThumbnailUrl = _storageUrlResolver.ResolvePublicUrl(lesson.ThumbnailImageUrl?.Path),
            VideoUrl = _storageUrlResolver.ResolvePublicUrl(lesson.VideoUrl?.Path),
            TranscriptUrl = _storageUrlResolver.ResolvePublicUrl(lesson.Transcript?.Path),
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Lesson, lessonContext).ToList()
        };
    }
}

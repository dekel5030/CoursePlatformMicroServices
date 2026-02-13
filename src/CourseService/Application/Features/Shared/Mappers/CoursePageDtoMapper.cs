using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;

namespace Courses.Application.Features.Shared.Mappers;

internal sealed class CoursePageDtoMapper : ICoursePageDtoMapper
{
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly ILinkBuilderService _linkBuilderService;

    public CoursePageDtoMapper(
        IStorageUrlResolver storageUrlResolver,
        ILinkBuilderService linkBuilderService)
    {
        _storageUrlResolver = storageUrlResolver;
        _linkBuilderService = linkBuilderService;
    }

    public CourseDto MapCourse(Course course, CourseContext context)
    {
        return new CourseDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            Description = course.Description.Value,
            Price = course.Price,
            Status = course.Status,
            ImageUrls = course.Images
                .Select(image => _storageUrlResolver.Resolve(StorageCategory.Public, image.Path).Value)
                .ToList(),
            Tags = course.Tags.Select(t => t.Value).ToList(),
            CategoryId = course.CategoryId.Value,
            InstructorId = course.InstructorId.Value,
            UpdatedAtUtc = course.UpdatedAtUtc,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Course, context).ToList()
        };
    }

    public LessonDto MapLesson(Lesson lesson, CourseContext courseContext, bool hasEnrollment)
    {
        var moduleContext = new ModuleContext(courseContext, lesson.ModuleId);
        var lessonContext = new LessonContext(moduleContext, lesson.Id, lesson.Access, hasEnrollment);

        return new LessonDto
        {
            Id = lesson.Id.Value,
            Title = lesson.Title.Value,
            Index = lesson.Index,
            Duration = lesson.Duration,
            ThumbnailUrl = lesson.ThumbnailImageUrl?.Path,
            Access = lesson.Access,
            ModuleId = lesson.ModuleId.Value,
            CourseId = lesson.CourseId.Value,
            Description = lesson.Description.Value,
            VideoUrl = lesson.VideoUrl?.Path,
            TranscriptUrl = lesson.Transcript?.Path,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Lesson, lessonContext).ToList()
        };
    }

    public ModuleDto MapModule(Module module, ModuleContext context)
    {
        return new ModuleDto
        {
            Id = module.Id.Value,
            Title = module.Title.Value,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Module, context).ToList()
        };
    }
}

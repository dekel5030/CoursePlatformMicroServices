using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;

namespace Courses.Application.Features.Management.ManagedCoursePage;

internal sealed class ManagedCoursePageComposer : IManagedCoursePageComposer
{
    private readonly ILinkProvider _linkProvider;
    private readonly CourseGovernancePolicy _policy;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public ManagedCoursePageComposer(
        ILinkProvider linkProvider,
        CourseGovernancePolicy policy,
        IStorageUrlResolver storageUrlResolver)
    {
        _linkProvider = linkProvider;
        _policy = policy;
        _storageUrlResolver = storageUrlResolver;
    }

    public ManagedCoursePageDto Compose(ManagedCoursePageData data)
    {
        var courseContext = new CourseContext(
            data.Course.Id,
            data.Course.InstructorId,
            data.Course.Status,
            IsManagementView: true);

        Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> moduleStats =
            CalculateModuleStats(data.Lessons);

        ManagedCourseLinks courseLinks = ResolveCourseLinks(data.Course.Id, courseContext);
        Dictionary<Guid, ManagedModuleLinks> moduleLinksMap = ResolveModuleLinks(data.Modules, courseContext);
        Dictionary<Guid, ManagedLessonLinks> lessonLinksMap = ResolveLessonLinks(data.Lessons, courseContext);

        ManagedCoursePageCourseDto courseDto = MapCourse(data.Course, courseLinks);
        Dictionary<Guid, ManagedCoursePageModuleDto> modulesDto = MapModules(
            data.Modules, moduleStats, moduleLinksMap);
        Dictionary<Guid, ManagedCoursePageLessonDto> lessonsDto = MapLessons(data.Lessons, lessonLinksMap);

        return new ManagedCoursePageDto
        {
            Course = courseDto,
            Structure = CourseStructureBuilder.Build(data.Modules, data.Lessons),
            Modules = modulesDto,
            Lessons = lessonsDto,
            Categories = MapCategories(data.Category)
        };
    }

    private static Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> CalculateModuleStats(
        IReadOnlyList<Lesson> lessons)
    {
        return lessons
            .GroupBy(lesson => lesson.ModuleId.Value)
            .ToDictionary(
                g => g.Key,
                g => (
                    LessonCount: g.Count(),
                    TotalDuration: TimeSpan.FromSeconds(g.Sum(l => l.Duration.TotalSeconds))
                ));
    }

    private ManagedCourseLinks ResolveCourseLinks(CourseId courseId, CourseContext courseContext)
    {
        Guid id = courseId.Value;
        bool canRead = _policy.CanReadCourse(courseContext);
        bool canEdit = _policy.CanEditCourse(courseContext);
        bool canDelete = _policy.CanDeleteCourse(courseContext);

        return new ManagedCourseLinks(
            Self: _linkProvider.GetManagedCourseLink(id),
            CoursePage: canRead ? _linkProvider.GetCoursePageLink(id) : null,
            Analytics: canEdit ? _linkProvider.GetCourseAnalyticsLink(id) : null,
            PartialUpdate: canEdit ? _linkProvider.GetPatchCourseLink(id) : null,
            Delete: canDelete ? _linkProvider.GetDeleteCourseLink(id) : null,
            Publish: canEdit ? _linkProvider.GetPublishCourseLink(id) : null,
            GenerateImageUploadUrl: canEdit ? _linkProvider.GetGenerateCourseImageUploadUrlLink(id) : null,
            CreateModule: canEdit ? _linkProvider.GetCreateModuleLink(id) : null,
            ReorderModules: canEdit ? _linkProvider.GetReorderModulesLink(id) : null);
    }

    private Dictionary<Guid, ManagedModuleLinks> ResolveModuleLinks(
        IReadOnlyList<Module> modules,
        CourseContext courseContext)
    {
        return modules.ToDictionary(
            module => module.Id.Value,
            module =>
            {
                var moduleContext = new ModuleContext(courseContext, module.Id);
                bool canEdit = _policy.CanEditModule(moduleContext);
                Guid moduleId = module.Id.Value;
                return new ManagedModuleLinks(
                    CreateLesson: canEdit ? _linkProvider.GetCreateLessonLink(moduleId) : null,
                    PartialUpdate: canEdit ? _linkProvider.GetPatchModuleLink(moduleId) : null,
                    Delete: canEdit ? _linkProvider.GetDeleteModuleLink(moduleId) : null,
                    ReorderLessons: canEdit ? _linkProvider.GetReorderLessonsLink(moduleId) : null);
            });
    }

    private Dictionary<Guid, ManagedLessonLinks> ResolveLessonLinks(
        IReadOnlyList<Lesson> lessons,
        CourseContext courseContext)
    {
        return lessons.ToDictionary(
            lesson => lesson.Id.Value,
            lesson =>
            {
                var moduleContext = new ModuleContext(courseContext, lesson.ModuleId);
                var lessonContext = new LessonContext(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);
                bool canEdit = _policy.CanEditLesson(lessonContext);
                Guid lessonId = lesson.Id.Value;
                return new ManagedLessonLinks(
                    Self: _linkProvider.GetLessonPageLink(lessonId),
                    PartialUpdate: canEdit ? _linkProvider.GetPatchLessonLink(lessonId) : null,
                    UploadVideoUrl: canEdit ? _linkProvider.GetLessonVideoUploadUrlLink(lessonId) : null,
                    AiGenerate: canEdit ? _linkProvider.GetGenerateLessonWithAiLink(lessonId) : null,
                    Move: canEdit ? _linkProvider.GetMoveLessonLink(lessonId) : null);
            });
    }

    private ManagedCoursePageCourseDto MapCourse(Course course, ManagedCourseLinks links)
    {
        var data = new CourseCoreDto(
            Id: course.Id.Value,
            Title: course.Title.Value,
            Description: course.Description.Value,
            Status: course.Status,
            Price: course.Price,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: course.Images
                .Select(image => _storageUrlResolver.Resolve(StorageCategory.Public, image.Path).Value)
                .ToList(),
            Tags: course.Tags.Select(t => t.Value).ToList(),
            InstructorId: course.InstructorId.Value,
            CategoryId: course.CategoryId.Value);
        return new ManagedCoursePageCourseDto(data, links);
    }

    private static Dictionary<Guid, ManagedCoursePageModuleDto> MapModules(
        IReadOnlyList<Module> modules,
        Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> stats,
        Dictionary<Guid, ManagedModuleLinks> linksMap)
    {
        return modules.ToDictionary(
            module => module.Id.Value,
            module =>
            {
                (int lessonCount, TimeSpan totalDuration) = stats.GetValueOrDefault(module.Id.Value);
                var moduleData = new ModuleCoreDto(
                    Id: module.Id.Value,
                    Title: module.Title.Value,
                    LessonCount: lessonCount,
                    Duration: totalDuration);
                return new ManagedCoursePageModuleDto(moduleData, linksMap[module.Id.Value]);
            });
    }

    private Dictionary<Guid, ManagedCoursePageLessonDto> MapLessons(
        IReadOnlyList<Lesson> lessons,
        Dictionary<Guid, ManagedLessonLinks> linksMap)
    {
        return lessons.ToDictionary(
            lesson => lesson.Id.Value,
            lesson =>
            {
                var lessonData = new LessonCoreDto(
                    Id: lesson.Id.Value,
                    Title: lesson.Title.Value,
                    Index: lesson.Index,
                    Duration: lesson.Duration,
                    ThumbnailUrl: _storageUrlResolver.ResolvePublicUrl(lesson.ThumbnailImageUrl?.Path),
                    Access: lesson.Access,
                    ModuleId: lesson.ModuleId.Value,
                    CourseId: lesson.CourseId.Value,
                    Description: lesson.Description.Value,
                    VideoUrl: _storageUrlResolver.ResolvePublicUrl(lesson.VideoUrl?.Path),
                    TranscriptUrl: _storageUrlResolver.ResolvePublicUrl(lesson.Transcript?.Path));
                return new ManagedCoursePageLessonDto(lessonData, linksMap[lesson.Id.Value]);
            });
    }

    private static Dictionary<Guid, CategoryDto> MapCategories(Category? category)
    {
        if (category == null)
        {
            return new Dictionary<Guid, CategoryDto>();
        }

        return new Dictionary<Guid, CategoryDto> { [category.Id.Value] = CategoryDtoMapper.Map(category) };
    }
}

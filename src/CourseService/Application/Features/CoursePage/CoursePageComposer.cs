using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Features.CoursePage;
using Courses.Application.Users.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.ReadModels;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Users;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Users;

namespace Courses.Application.Features.CoursePage;

internal sealed class CoursePageComposer : ICoursePageComposer
{
    private readonly ILinkProvider _linkProvider;
    private readonly CourseGovernancePolicy _policy;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public CoursePageComposer(
        ILinkProvider linkProvider,
        CourseGovernancePolicy policy,
        IStorageUrlResolver storageUrlResolver)
    {
        _linkProvider = linkProvider;
        _policy = policy;
        _storageUrlResolver = storageUrlResolver;
    }

    public CoursePageDto Compose(CoursePageData data, CourseAnalytics? analytics)
    {
        var courseContext = new CourseContext(
            data.Course.Id,
            data.Course.InstructorId,
            data.Course.Status,
            IsManagementView: false);

        Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> moduleStats =
            CalculateModuleStats(data.Lessons);

        CoursePageCourseLinks courseLinks = ResolveCourseLinks(data.Course.Id.Value, courseContext);
        Dictionary<Guid, CoursePageModuleLinks> moduleLinksMap = ResolveModuleLinks(data.Modules, courseContext);
        Dictionary<Guid, CoursePageLessonLinks> lessonLinksMap = ResolveLessonLinks(data.Lessons, courseContext);

        CoursePageCourseDto courseDto = MapCourse(data.Course, courseLinks);
        Dictionary<Guid, CoursePageModuleDto> modulesDto = MapModules(data.Modules, moduleStats, moduleLinksMap);
        Dictionary<Guid, CoursePageLessonDto> lessonsDto = MapLessons(data.Lessons, lessonLinksMap);

        return new CoursePageDto
        {
            Course = courseDto,
            Analytics = CourseAnalyticsDtoMapper.ToCourseAnalytics(analytics),
            Structure = CourseStructureBuilder.Build(data.Modules, data.Lessons),
            Modules = modulesDto,
            Lessons = lessonsDto,
            Instructors = MapInstructors(data.Instructor),
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
                    TotalDuration: TimeSpan.FromSeconds(g.Sum(l => l.Duration.TotalSeconds))));
    }

    private CoursePageCourseLinks ResolveCourseLinks(Guid courseId, CourseContext courseContext)
    {
        bool canEdit = _policy.CanEditCourse(courseContext);
        return new CoursePageCourseLinks(
            Self: _linkProvider.GetCoursePageLink(courseId),
            Manage: canEdit ? _linkProvider.GetManagedCourseLink(courseId) : null,
            Analytics: canEdit ? _linkProvider.GetCourseAnalyticsLink(courseId) : null,
            Ratings: _linkProvider.GetCourseRatingsLink(courseId));
    }

    private Dictionary<Guid, CoursePageModuleLinks> ResolveModuleLinks(
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
                return new CoursePageModuleLinks(
                    CreateLesson: canEdit ? _linkProvider.GetCreateLessonLink(moduleId) : null,
                    PartialUpdate: canEdit ? _linkProvider.GetPatchModuleLink(moduleId) : null,
                    Delete: canEdit ? _linkProvider.GetDeleteModuleLink(moduleId) : null,
                    ReorderLessons: canEdit ? _linkProvider.GetReorderLessonsLink(moduleId) : null);
            });
    }

    private Dictionary<Guid, CoursePageLessonLinks> ResolveLessonLinks(
        IReadOnlyList<Lesson> lessons,
        CourseContext courseContext)
    {
        return lessons.ToDictionary(
            lesson => lesson.Id.Value,
            lesson =>
            {
                var moduleContext = new ModuleContext(courseContext, lesson.ModuleId);
                var lessonContext = new LessonContext(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);
                bool canRead = _policy.CanReadLesson(lessonContext);
                bool canEdit = _policy.CanEditLesson(lessonContext);
                Guid lessonId = lesson.Id.Value;
                return new CoursePageLessonLinks(
                    Self: canRead ? _linkProvider.GetLessonPageLink(lessonId) : null,
                    PartialUpdate: canEdit ? _linkProvider.GetPatchLessonLink(lessonId) : null,
                    UploadVideoUrl: canEdit ? _linkProvider.GetLessonVideoUploadUrlLink(lessonId) : null,
                    AiGenerate: canEdit ? _linkProvider.GetGenerateLessonWithAiLink(lessonId) : null,
                    Move: canEdit ? _linkProvider.GetMoveLessonLink(lessonId) : null);
            });
    }

    private CoursePageCourseDto MapCourse(Course course, CoursePageCourseLinks links)
    {
        var data = new CoursePageCourseData(
            Id: course.Id.Value,
            Title: course.Title.Value,
            Description: course.Description.Value,
            Price: course.Price,
            Status: course.Status,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: course.Images
                .Select(image => _storageUrlResolver.Resolve(StorageCategory.Public, image.Path).Value)
                .ToList(),
            Tags: course.Tags.Select(t => t.Value).ToList(),
            InstructorId: course.InstructorId.Value,
            CategoryId: course.CategoryId.Value);
        return new CoursePageCourseDto(data, links);
    }

    private static Dictionary<Guid, CoursePageModuleDto> MapModules(
        IReadOnlyList<Module> modules,
        Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> stats,
        Dictionary<Guid, CoursePageModuleLinks> linksMap)
    {
        return modules.ToDictionary(
            module => module.Id.Value,
            module =>
            {
                (int lessonCount, TimeSpan totalDuration) = stats.GetValueOrDefault(module.Id.Value);
                var moduleData = new CoursePageModuleData(
                    Id: module.Id.Value,
                    Title: module.Title.Value,
                    LessonCount: lessonCount,
                    TotalDuration: totalDuration);
                return new CoursePageModuleDto(moduleData, linksMap[module.Id.Value]);
            });
    }

    private Dictionary<Guid, CoursePageLessonDto> MapLessons(
        IReadOnlyList<Lesson> lessons,
        Dictionary<Guid, CoursePageLessonLinks> linksMap)
    {
        return lessons.ToDictionary(
            lesson => lesson.Id.Value,
            lesson =>
            {
                var lessonData = new CoursePageLessonData(
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
                return new CoursePageLessonDto(lessonData, linksMap[lesson.Id.Value]);
            });
    }

    private static Dictionary<Guid, UserDto> MapInstructors(User? instructor)
    {
        if (instructor == null)
        {
            return new Dictionary<Guid, UserDto>();
        }

        return new Dictionary<Guid, UserDto> { [instructor.Id.Value] = UserDtoMapper.Map(instructor) };
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

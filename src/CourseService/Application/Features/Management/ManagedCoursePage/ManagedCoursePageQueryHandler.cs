using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Management.ManagedCoursePage.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedCoursePage;

internal sealed class ManagedCoursePageQueryHandler
    : IQueryHandler<ManagedCoursePageQuery, ManagedCoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IUserContext _userContext;
    private readonly CourseGovernancePolicy _policy;
    private readonly ILinkProvider _linkProvider;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public ManagedCoursePageQueryHandler(
        IReadDbContext readDbContext,
        IUserContext userContext,
        CourseGovernancePolicy policy,
        ILinkProvider linkProvider,
        IStorageUrlResolver storageUrlResolver)
    {
        _readDbContext = readDbContext;
        _userContext = userContext;
        _policy = policy;
        _linkProvider = linkProvider;
        _storageUrlResolver = storageUrlResolver;
    }

    public async Task<Result<ManagedCoursePageDto>> Handle(
        ManagedCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        ManagedCoursePageData? data = await LoadCoursePageDataAsync(courseId, cancellationToken);
        if (data == null)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.NotFound);
        }

        Result<UserId> authResult = InstructorAuthorization
            .EnsureInstructorAuthorized(_userContext, data.Course.InstructorId);
        if (authResult.IsFailure)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.Unauthorized);
        }

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

        return Result.Success(new ManagedCoursePageDto
        {
            Course = courseDto,
            Structure = CourseStructureBuilder.Build(data.Modules, data.Lessons),
            Modules = modulesDto,
            Lessons = lessonsDto,
            Categories = MapCategories(data.Category)
        });
    }

    private async Task<ManagedCoursePageData?> LoadCoursePageDataAsync(
        CourseId courseId,
        CancellationToken cancellationToken)
    {
        var raw = await _readDbContext.Courses
            .AsSplitQuery()
            .Where(c => c.Id == courseId)
            .Select(course => new
            {
                Course = course,
                Modules = _readDbContext.Modules
                    .Where(m => m.CourseId == course.Id)
                    .OrderBy(m => m.Index)
                    .ToList(),
                Lessons = _readDbContext.Lessons
                    .Where(l => l.CourseId == course.Id)
                    .OrderBy(l => l.Index)
                    .ToList(),
                Category = _readDbContext.Categories.FirstOrDefault(cat => cat.Id == course.CategoryId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw == null)
        {
            return null;
        }

        return new ManagedCoursePageData(raw.Course, raw.Modules, raw.Lessons, raw.Category);
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
        return new ManagedCoursePageCourseDto
        {
            Course = new CourseDto
            {
                Id = course.Id.Value,
                Title = course.Title.Value,
                Description = course.Description.Value,
                Status = course.Status,
                Price = course.Price,
                UpdatedAtUtc = course.UpdatedAtUtc,
                ImageUrls = course.Images
                .Select(image => _storageUrlResolver.Resolve(StorageCategory.Public, image.Path).Value)
                .ToList(),
                Tags = course.Tags.Select(t => t.Value).ToList(),
                InstructorId = course.InstructorId.Value,
                CategoryId = course.CategoryId.Value,
                Links = []
            },
            Links = links
        };
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
                return new ManagedCoursePageModuleDto
                {
                    Id = module.Id.Value,
                    Title = module.Title.Value,
                    LessonCount = lessonCount,
                    Duration = totalDuration,
                    Links = linksMap[module.Id.Value]
                };
            });
    }

    private static Dictionary<Guid, ManagedCoursePageLessonDto> MapLessons(
        IReadOnlyList<Lesson> lessons,
        Dictionary<Guid, ManagedLessonLinks> linksMap)
    {
        return lessons.ToDictionary(
            lesson => lesson.Id.Value,
            lesson => new ManagedCoursePageLessonDto
            {
                Lesson = new LessonDto 
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
                    Links = [],
                },
                Links = linksMap[lesson.Id.Value]
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

    private sealed record ManagedCoursePageData(
        Course Course,
        List<Module> Modules,
        List<Lesson> Lessons,
        Category? Category);
}

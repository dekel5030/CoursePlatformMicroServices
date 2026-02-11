using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetManagedCourse;

internal sealed class GetManagedCourseQueryHandler
    : IQueryHandler<GetManagedCourseQuery, ManagedCoursePageDto>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ILinkBuilderService _linkBuilderService;
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly IUserContext _userContext;

    public GetManagedCourseQueryHandler(
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

    public async Task<Result<ManagedCoursePageDto>> Handle(
        GetManagedCourseQuery request,
        CancellationToken cancellationToken = default)
    {
        if (_userContext.Id is null || !_userContext.IsAuthenticated)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.Unauthorized);
        }

        var courseId = new CourseId(request.Id);
        var instructorId = new UserId(_userContext.Id.Value);

        var courseData = await _writeDbContext.Courses
            .AsNoTracking()
            .AsSplitQuery()
            .Where(c => c.Id == courseId)
            .Select(course => new
            {
                Course = course,
                Modules = _writeDbContext.Modules
                    .Where(m => m.CourseId == course.Id)
                    .OrderBy(m => m.Index)
                    .ToList(),
                Lessons = _writeDbContext.Lessons
                    .Where(l => l.CourseId == course.Id)
                    .OrderBy(l => l.Index)
                    .ToList(),
                Category = _writeDbContext.Categories.FirstOrDefault(cat => cat.Id == course.CategoryId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (courseData == null)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.NotFound);
        }

        if (courseData.Course.InstructorId != instructorId)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.Unauthorized);
        }

        CourseContext courseContext = new(courseData.Course.Id, courseData.Course.InstructorId, courseData.Course.Status, IsManagementView: true);

        CourseDto courseDto = MapToCourseDto(courseData.Course, courseContext);
        CourseStructureDto structure = BuildStructure(courseData.Modules, courseData.Lessons);

        IReadOnlyDictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> moduleStatsByModuleId = courseData.Lessons
            .GroupBy(l => l.ModuleId.Value)
            .ToDictionary(g => g.Key, g => (
                LessonCount: g.Count(),
                TotalDuration: TimeSpan.FromSeconds(g.Sum(l => l.Duration.TotalSeconds))
            ));

        return Result.Success(new ManagedCoursePageDto
        {
            Course = courseDto,
            Structure = structure,

            Modules = courseData.Modules.ToDictionary(
                m => m.Id.Value,
                m => MapToModuleDto(m, courseContext, moduleStatsByModuleId)),

            Lessons = courseData.Lessons.ToDictionary(
                l => l.Id.Value,
                l => MapToLessonDto(l, courseData.Course.Title, courseContext, false)),

            Categories = courseData.Category != null
                ? new Dictionary<Guid, CategoryDto> { [courseData.Category.Id.Value] = MapToCategoryDto(courseData.Category) }
                : new()
        });
    }

    private CourseDto MapToCourseDto(Course course, CourseContext context)
    {
        return new CourseDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            Description = course.Description.Value,
            Price = course.Price,
            Status = course.Status,
            ImageUrls = course.Images.Select(image => _storageUrlResolver.Resolve(StorageCategory.Public, image.Path).Value).ToList(),
            Tags = course.Tags.Select(t => t.Value).ToList(),
            CategoryId = course.CategoryId.Value,
            InstructorId = course.InstructorId.Value,
            UpdatedAtUtc = course.UpdatedAtUtc,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Course, context).ToList()
        };
    }

    private static CourseStructureDto BuildStructure(
        IReadOnlyList<Module> modules,
        IReadOnlyList<Lesson> lessons)
    {
        var orderedModules = modules.OrderBy(m => m.Index).ToList();
        var moduleIds = orderedModules.Select(m => m.Id.Value).ToList();
        var moduleLessonIds = orderedModules.ToDictionary(
            m => m.Id.Value,
            m => (IReadOnlyList<Guid>)lessons
                .Where(l => l.ModuleId == m.Id)
                .OrderBy(l => l.Index)
                .Select(l => l.Id.Value)
                .ToList());
        return new CourseStructureDto
        {
            ModuleIds = moduleIds,
            ModuleLessonIds = moduleLessonIds
        };
    }

    private ModuleWithAnalyticsDto MapToModuleDto(
        Module module,
        CourseContext courseContext,
        IReadOnlyDictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> moduleStatsByModuleId)
    {
        var moduleContext = new ModuleContext(courseContext, module.Id);

        var moduleDto = new ModuleDto
        {
            Id = module.Id.Value,
            Title = module.Title.Value,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Module, moduleContext).ToList()
        };

        (int lessonCount, TimeSpan totalDuration) = moduleStatsByModuleId.GetValueOrDefault(module.Id.Value);
        ModuleAnalyticsDto analyticsDto = new(
            lessonCount,
            totalDuration);

        return new ModuleWithAnalyticsDto(moduleDto, analyticsDto);
    }

    private LessonDto MapToLessonDto(Lesson lesson, Title courseTitle, CourseContext courseContext, bool hasEnrollment)
    {
        var moduleContext = new ModuleContext(courseContext, lesson.ModuleId);
        var lessonContext = new LessonContext(moduleContext, lesson.Id, lesson.Access, hasEnrollment);

        return new()
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
            CourseName = courseTitle.Value,
            TranscriptUrl = lesson.Transcript?.Path,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Lesson, lessonContext).ToList()
        };
    }

    private static CategoryDto MapToCategoryDto(Category category)
    {
        return new(category.Id.Value, category.Name, category.Slug.Value);
    }
}

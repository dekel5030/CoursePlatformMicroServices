using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Shared.Primitives;
using Courses.Domain.Users;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCoursePage;

internal sealed class GetCoursePageQueryHandler
    : IQueryHandler<GetCoursePageQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILinkBuilderService _linkBuilderService;
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly IImmediateEventBus _immediateEventBus;
    private readonly IUserContext _userContext;

    public GetCoursePageQueryHandler(
        IReadDbContext readDbContext,
        ILinkBuilderService linkBuilderService,
        IStorageUrlResolver storageUrlResolver,
        IImmediateEventBus immediateEventBus,
        IUserContext userContext)
    {
        _readDbContext = readDbContext;
        _linkBuilderService = linkBuilderService;
        _storageUrlResolver = storageUrlResolver;
        _immediateEventBus = immediateEventBus;
        _userContext = userContext;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        var courseData = await _readDbContext.Courses
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
                Instructor = _readDbContext.Users.FirstOrDefault(u => u.Id == course.InstructorId),
                Category = _readDbContext.Categories.FirstOrDefault(cat => cat.Id == course.CategoryId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (courseData == null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        CourseAnalytics? analytics = await _readDbContext.CourseAnalytics
            .FirstOrDefaultAsync(c => c.CourseId == courseId.Value, cancellationToken);

        CourseContext courseContext = new(courseData.Course.Id, courseData.Course.InstructorId, courseData.Course.Status);

        CourseDto courseDto = MapToCourseDto(courseData.Course, courseContext);
        CourseAnalyticsDto analyticsDto = analytics != null
            ? new CourseAnalyticsDto(
                analytics.EnrollmentsCount,
                analytics.TotalLessonsCount,
                analytics.TotalCourseDuration,
                analytics.AverageRating,
                analytics.ReviewsCount,
                analytics.ViewCount)
            : new CourseAnalyticsDto(0, 0, TimeSpan.Zero, 0, 0, 0);

        CourseStructureDto structure = BuildStructure(courseData.Modules, courseData.Lessons);

        await _immediateEventBus.PublishAsync(
            new CourseViewedIntegrationEvent(request.Id, _userContext.Id, DateTimeOffset.UtcNow),
            cancellationToken);

        Dictionary<Guid, ReadModels.ModuleAnalytics> moduleAnalyticsByModuleId = analytics?.ModuleAnalytics?.ToDictionary(ma => ma.ModuleId) ?? new Dictionary<Guid, ReadModels.ModuleAnalytics>();

        return Result.Success(new CoursePageDto
        {
            Course = courseDto,
            Analytics = analyticsDto,
            Structure = structure,

            Modules = courseData.Modules.ToDictionary(
                m => m.Id.Value,
                m => MapToModuleDto(m, courseContext, moduleAnalyticsByModuleId)),

            Lessons = courseData.Lessons.ToDictionary(
                l => l.Id.Value,
                l => MapToLessonDto(l, courseData.Course.Title, courseContext, false)),

            Instructors = courseData.Instructor != null
                ? new Dictionary<Guid, UserDto> { [courseData.Instructor.Id.Value] = MapToUserDto(courseData.Instructor) }
                : new(),

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
        IReadOnlyDictionary<Guid, ReadModels.ModuleAnalytics> moduleAnalyticsByModuleId)
    {
        var moduleContext = new ModuleContext(courseContext, module.Id);

        var moduleDto = new ModuleDto
        {
            Id = module.Id.Value,
            Title = module.Title.Value,
            Links = _linkBuilderService.BuildLinks(LinkResourceKey.Module, moduleContext).ToList()
        };

        ReadModels.ModuleAnalytics? analytics = moduleAnalyticsByModuleId.GetValueOrDefault(module.Id.Value);
        ModuleAnalyticsDto analyticsDto = new(
            analytics?.LessonCount ?? 0,
            analytics?.TotalModuleDuration ?? TimeSpan.Zero);

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

    private static UserDto MapToUserDto(User user)
    {
        return new()
        {
            Id = user.Id.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarUrl = user.AvatarUrl
        };
    }

    private static CategoryDto MapToCategoryDto(Category category)
    {
        return new(category.Id.Value, category.Name, category.Slug.Value);
    }
}

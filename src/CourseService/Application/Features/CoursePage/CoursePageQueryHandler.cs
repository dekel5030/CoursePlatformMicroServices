using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Users;
using Courses.Application.Users.Dtos;
using Courses.Domain.Categories;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Users;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CoursePage;

internal sealed class CoursePageQueryHandler
    : IQueryHandler<CoursePageQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ICoursePageDataLoader _coursePageDataLoader;
    private readonly ICoursePageDtoMapper _coursePageDtoMapper;
    private readonly IImmediateEventBus _immediateEventBus;
    private readonly IUserContext _userContext;

    public CoursePageQueryHandler(
        IReadDbContext readDbContext,
        ICoursePageDataLoader coursePageDataLoader,
        ICoursePageDtoMapper coursePageDtoMapper,
        IImmediateEventBus immediateEventBus,
        IUserContext userContext)
    {
        _readDbContext = readDbContext;
        _coursePageDataLoader = coursePageDataLoader;
        _coursePageDtoMapper = coursePageDtoMapper;
        _immediateEventBus = immediateEventBus;
        _userContext = userContext;
    }

    public async Task<Result<CoursePageDto>> Handle(
        CoursePageQuery request, 
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        CoursePageData? courseData = await _coursePageDataLoader.LoadAsync(courseId, cancellationToken);
        
        if (courseData == null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        CourseAnalytics? analytics = await FetchAnalyticsAsync(courseId, cancellationToken);

        await PublishViewedEventAsync(request.Id, cancellationToken);

        var courseContext = new CourseContext(
            courseData.Course.Id, 
            courseData.Course.InstructorId, 
            courseData.Course.Status, 
            IsManagementView: false);

        return Result.Success(new CoursePageDto
        {
            Course = _coursePageDtoMapper.MapCourse(courseData.Course, courseContext),
            Analytics = CourseAnalyticsDtoMapper.ToCourseAnalytics(analytics),
            Structure = CourseStructureBuilder.Build(courseData.Modules, courseData.Lessons),

            Modules = MapModules(courseData.Modules, analytics, courseContext),
            Lessons = MapLessons(courseData.Lessons, courseContext),

            Instructors = MapInstructors(courseData.Instructor),
            Categories = MapCategories(courseData.Category)
        });
    }

    private async Task<CourseAnalytics?> FetchAnalyticsAsync(
        CourseId courseId,
        CancellationToken cancellationToken = default)
    {
        return await _readDbContext.CourseAnalytics
            .FirstOrDefaultAsync(course => course.CourseId == courseId.Value, cancellationToken);
    }

    private async Task PublishViewedEventAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        await _immediateEventBus.PublishAsync(
            new CourseViewedIntegrationEvent(courseId, _userContext.Id, DateTimeOffset.UtcNow),
            cancellationToken);
    }

    private Dictionary<Guid, ModuleWithAnalyticsDto> MapModules(
        IReadOnlyList<Module> modules,
        CourseAnalytics? analytics,
        CourseContext courseContext)
    {
        Dictionary<Guid, ModuleAnalytics> moduleAnalyticsLookup = analytics?.ModuleAnalytics?
            .ToDictionary(ma => ma.ModuleId) ?? new Dictionary<Guid, ModuleAnalytics>();

        return modules.ToDictionary(
            module => module.Id.Value,
            module =>
            {
                var moduleContext = new ModuleContext(courseContext, module.Id);
                ModuleDto moduleDto = _coursePageDtoMapper.MapModule(module, moduleContext);

                moduleAnalyticsLookup.TryGetValue(module.Id.Value, out ModuleAnalytics? ma);
                var analyticsDto = new ModuleAnalyticsDto(
                    ma?.LessonCount ?? 0,
                    ma?.TotalModuleDuration ?? TimeSpan.Zero);

                return new ModuleWithAnalyticsDto(moduleDto, analyticsDto);
            });
    }

    private Dictionary<Guid, LessonDto> MapLessons(IReadOnlyList<Lesson> lessons, CourseContext context)
    {
        return lessons.ToDictionary(
            lesson => lesson.Id.Value,
            lesson => _coursePageDtoMapper.MapLesson(lesson, context, false));
    }

    private static Dictionary<Guid, UserDto> MapInstructors(User? instructor)
    {
        if (instructor == null)
        {
            return new();
        }

        return new Dictionary<Guid, UserDto> { [instructor.Id.Value] = UserDtoMapper.Map(instructor) };
    }

    private static Dictionary<Guid, CategoryDto> MapCategories(Category? category)
    {
        if (category == null)
        {
            return new();
        }

        return new Dictionary<Guid, CategoryDto> { [category.Id.Value] = CategoryDtoMapper.Map(category) };
    }
}

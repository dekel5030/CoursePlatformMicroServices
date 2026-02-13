using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Modules.Dtos;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Users;
using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
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

        CourseAnalytics? analytics = await _readDbContext.CourseAnalytics
            .FirstOrDefaultAsync(c => c.CourseId == courseId.Value, cancellationToken);

        CourseContext courseContext = new(courseData.Course.Id, courseData.Course.InstructorId, courseData.Course.Status, IsManagementView: false);

        CourseDto courseDto = _coursePageDtoMapper.MapCourse(courseData.Course, courseContext);
        CourseAnalyticsDto analyticsDto = CourseAnalyticsDtoMapper.ToCourseAnalytics(analytics);

        CourseStructureDto structure = CourseStructureBuilder.Build(courseData.Modules, courseData.Lessons);

        await _immediateEventBus.PublishAsync(
            new CourseViewedIntegrationEvent(request.Id, _userContext.Id, DateTimeOffset.UtcNow),
            cancellationToken);

        Dictionary<Guid, ModuleAnalytics> moduleAnalyticsByModuleId = analytics?.ModuleAnalytics?.ToDictionary(ma => ma.ModuleId) ?? new Dictionary<Guid, ModuleAnalytics>();

        return Result.Success(new CoursePageDto
        {
            Course = courseDto,
            Analytics = analyticsDto,
            Structure = structure,

            Modules = courseData.Modules.ToDictionary(
                m => m.Id.Value,
                m =>
                {
                    var moduleContext = new ModuleContext(courseContext, m.Id);
                    ModuleDto moduleDto = _coursePageDtoMapper.MapModule(m, moduleContext);
                    ModuleAnalytics? ma = moduleAnalyticsByModuleId.GetValueOrDefault(m.Id.Value);
                    var moduleAnalyticsDto = new ModuleAnalyticsDto(
                        ma?.LessonCount ?? 0,
                        ma?.TotalModuleDuration ?? TimeSpan.Zero);
                    return new ModuleWithAnalyticsDto(moduleDto, moduleAnalyticsDto);
                }),

            Lessons = courseData.Lessons.ToDictionary(
                l => l.Id.Value,
                lesson => _coursePageDtoMapper.MapLesson(lesson, courseContext, false)),

            Instructors = courseData.Instructor != null
                ? new Dictionary<Guid, UserDto> { [courseData.Instructor.Id.Value] = UserDtoMapper.Map(courseData.Instructor) }
                : new(),

            Categories = courseData.Category != null
                ? new Dictionary<Guid, CategoryDto> { [courseData.Category.Id.Value] = CategoryDtoMapper.Map(courseData.Category) }
                : new()
        });
    }
}

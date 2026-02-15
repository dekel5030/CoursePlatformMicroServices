using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Categories;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.Management.ManagedCoursePage;

internal sealed class ManagedCoursePageQueryHandler
    : IQueryHandler<ManagedCoursePageQuery, ManagedCoursePageDto>
{
    private readonly ICoursePageDataLoader _coursePageDataLoader;
    private readonly ICoursePageDtoMapper _coursePageDtoMapper;
    private readonly IUserContext _userContext;

    public ManagedCoursePageQueryHandler(
        ICoursePageDataLoader coursePageDataLoader,
        ICoursePageDtoMapper coursePageDtoMapper,
        IUserContext userContext)
    {
        _coursePageDataLoader = coursePageDataLoader;
        _coursePageDtoMapper = coursePageDtoMapper;
        _userContext = userContext;
    }

    public async Task<Result<ManagedCoursePageDto>> Handle(
        ManagedCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        CoursePageData? courseData = await _coursePageDataLoader.LoadAsync(courseId, cancellationToken);
        if (courseData == null)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.NotFound);
        }

        Result<UserId> authResult = InstructorAuthorization
            .EnsureInstructorAuthorized(_userContext, courseData.Course.InstructorId);
        if (authResult.IsFailure)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.Unauthorized);
        }

        var courseContext = new CourseContext(
            courseData.Course.Id, 
            courseData.Course.InstructorId, 
            courseData.Course.Status, 
            IsManagementView: true);

        Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> moduleStats = 
            CalculateModuleStats(courseData.Lessons);

        return Result.Success(new ManagedCoursePageDto
        {
            Course = _coursePageDtoMapper.MapCourse(courseData.Course, courseContext),
            Structure = CourseStructureBuilder.Build(courseData.Modules, courseData.Lessons),

            Modules = MapManagedModules(courseData.Modules, moduleStats, courseContext),
            Lessons = MapLessons(courseData.Lessons, courseContext),
            Categories = MapCategories(courseData.Category)
        });
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

    private Dictionary<Guid, ManagedModuleDto> MapManagedModules(
        IReadOnlyList<Module> modules,
        Dictionary<Guid, (int LessonCount, TimeSpan TotalDuration)> stats,
        CourseContext courseContext)
    {
        return modules.ToDictionary(
            module => module.Id.Value,
            module =>
            {
                var moduleContext = new ModuleContext(courseContext, module.Id);
                ModuleDto moduleDto = _coursePageDtoMapper.MapModule(module, moduleContext);

                (int lessonCount, TimeSpan totalDuration) = stats.GetValueOrDefault(module.Id.Value);
                var statsDto = new ManagedModuleStatsDto(lessonCount, totalDuration);

                return new ManagedModuleDto(moduleDto, statsDto);
            });
    }

    private Dictionary<Guid, LessonDto> MapLessons(IReadOnlyList<Lesson> lessons, CourseContext context)
    {
        return lessons.ToDictionary(
            l => l.Id.Value,
            l => _coursePageDtoMapper.MapLesson(l, context, false));
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

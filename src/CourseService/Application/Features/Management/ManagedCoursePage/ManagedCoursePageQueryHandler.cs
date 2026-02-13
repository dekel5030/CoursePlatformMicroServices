using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Modules.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
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

        Result<UserId> authResult = InstructorAuthorization.EnsureInstructorAuthorized(
            _userContext,
            courseData.Course.InstructorId);

        if (authResult.IsFailure)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.Unauthorized);
        }

        CourseContext courseContext = new(courseData.Course.Id, courseData.Course.InstructorId, courseData.Course.Status, IsManagementView: true);

        CourseDto courseDto = _coursePageDtoMapper.MapCourse(courseData.Course, courseContext);
        CourseStructureDto structure = CourseStructureBuilder.Build(courseData.Modules, courseData.Lessons);

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
                m =>
                {
                    var moduleContext = new ModuleContext(courseContext, m.Id);
                    ModuleDto moduleDto = _coursePageDtoMapper.MapModule(m, moduleContext);
                    (int lessonCount, TimeSpan totalDuration) = moduleStatsByModuleId.GetValueOrDefault(m.Id.Value);
                    return new ManagedModuleDto(moduleDto, new ManagedModuleStatsDto(lessonCount, totalDuration));
                }),

            Lessons = courseData.Lessons.ToDictionary(
                l => l.Id.Value,
                l => _coursePageDtoMapper.MapLesson(l, courseContext, false)),

            Categories = courseData.Category != null
                ? new Dictionary<Guid, CategoryDto> { [courseData.Category.Id.Value] = CategoryDtoMapper.Map(courseData.Category) }
                : new()
        });
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Application.Categories;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Modules.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedCoursePage;

internal sealed class ManagedCoursePageQueryHandler
    : IQueryHandler<ManagedCoursePageQuery, ManagedCoursePageDto>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ICoursePageDtoMapper _coursePageDtoMapper;
    private readonly IUserContext _userContext;

    public ManagedCoursePageQueryHandler(
        IWriteDbContext writeDbContext,
        ICoursePageDtoMapper coursePageDtoMapper,
        IUserContext userContext)
    {
        _writeDbContext = writeDbContext;
        _coursePageDtoMapper = coursePageDtoMapper;
        _userContext = userContext;
    }

    public async Task<Result<ManagedCoursePageDto>> Handle(
        ManagedCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

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

using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Modules.Queries.GetModules;

internal sealed class GetModulesQueryHandler : IQueryHandler<GetModulesQuery, IReadOnlyList<ModuleWithAnalyticsAndStructureDto>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILinkBuilderService _linkBuilder;

    public GetModulesQueryHandler(IReadDbContext readDbContext, ILinkBuilderService linkBuilder)
    {
        _readDbContext = readDbContext;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<IReadOnlyList<ModuleWithAnalyticsAndStructureDto>>> Handle(
        GetModulesQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Module> query = _readDbContext.Modules.AsNoTracking();

        if (request.Filter.CourseId is { } courseId)
        {
            query = query.Where(m => m.CourseId == courseId);
        }

        if (request.Filter.Ids is { } idsEnumerable)
        {
            var ids = idsEnumerable.Distinct().Select(id => new ModuleId(id)).ToList();
            if (ids.Count > 0)
            {
                query = query.Where(m => ids.Contains(m.Id));
            }
        }

        List<Module> modules = await query
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);

        if (modules.Count == 0)
        {
            return Result.Success<IReadOnlyList<ModuleWithAnalyticsAndStructureDto>>([]);
        }

        var moduleIds = modules.Select(m => m.Id).ToList();
        List<Lesson> lessonsGroupedByModule = await _readDbContext.Lessons
            .AsNoTracking()
            .Where(l => moduleIds.Contains(l.ModuleId))
            .OrderBy(l => l.Index)
            .ToListAsync(cancellationToken);

        var lessonsByModuleId = lessonsGroupedByModule
            .GroupBy(l => l.ModuleId)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<Guid, ReadModels.ModuleAnalytics>? moduleAnalyticsByModuleId = null;
        if (request.Filter.CourseId is { } singleCourseId)
        {
            CourseAnalytics? courseAnalytics = await _readDbContext.CourseAnalytics
                .FirstOrDefaultAsync(c => c.CourseId == singleCourseId.Value, cancellationToken);
            moduleAnalyticsByModuleId = courseAnalytics?.ModuleAnalytics?.ToDictionary(ma => ma.ModuleId);
        }

        var courseIds = modules.Select(m => m.CourseId).Distinct().ToList();
        List<Course> courses = await _readDbContext.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
        var coursesById = courses.ToDictionary(c => c.Id);

        var moduleDetailsDtos = modules.Select(module =>
        {
            List<Lesson> moduleLessons = lessonsByModuleId.TryGetValue(module.Id, out List<Lesson>? lessons)
                ? lessons
                : [];

            var lessonDtos = moduleLessons
                .Select(lesson => new LessonSummaryDto(
                    module.Id.Value,
                    lesson.Id.Value,
                    lesson.Title.Value,
                    lesson.Index,
                    lesson.Duration,
                    null,
                    lesson.Access
                )).ToList();

            Course? course = coursesById.GetValueOrDefault(module.CourseId);
            var moduleDto = new ModuleDto
            {
                Id = module.Id.Value,
                Title = module.Title.Value,
                Links = course is not null
                    ? _linkBuilder.BuildLinks(LinkResourceKey.Module, new ModuleContext(
                        new CourseContext(course.Id, course.InstructorId, course.Status), module.Id)).ToList()
                    : []
            };

            ModuleAnalyticsDto analyticsDto = moduleAnalyticsByModuleId is { } byId && byId.TryGetValue(module.Id.Value, out ReadModels.ModuleAnalytics? ma)
                ? new ModuleAnalyticsDto(ma.LessonCount, ma.TotalModuleDuration)
                : new ModuleAnalyticsDto(
                    lessonDtos.Count,
                    TimeSpan.FromTicks(lessonDtos.Sum(l => l.Duration.Ticks)));

            var lessonIds = lessonDtos.Select(l => l.LessonId).ToList();

            return new ModuleWithAnalyticsAndStructureDto(moduleDto, analyticsDto, lessonIds);
        }).ToList();

        return Result.Success<IReadOnlyList<ModuleWithAnalyticsAndStructureDto>>(moduleDetailsDtos);
    }
}

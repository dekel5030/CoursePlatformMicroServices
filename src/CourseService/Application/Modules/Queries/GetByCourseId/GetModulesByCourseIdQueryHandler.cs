using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Modules.Queries.GetByCourseId;

internal sealed class GetModulesByCourseIdQueryHandler
    : IQueryHandler<GetModulesByCourseIdQuery, IReadOnlyList<ModuleDto>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILinkBuilderService _linkBuilder;

    public GetModulesByCourseIdQueryHandler(IReadDbContext readDbContext, ILinkBuilderService linkBuilder)
    {
        _readDbContext = readDbContext;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<IReadOnlyList<ModuleDto>>> Handle(
        GetModulesByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(course => course.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<IReadOnlyList<ModuleDto>>(CourseErrors.NotFound);
        }

        List<Module> modules = await _readDbContext.Modules
            .Where(module => module.CourseId.Value == request.CourseId.Value)
            .OrderBy(module => module.Index)
            .ToListAsync(cancellationToken);

        if (modules.Count == 0)
        {
            return Result.Success<IReadOnlyList<ModuleDto>>([]);
        }

        var moduleIds = modules.Select(m => m.Id).ToList();
        List<Lesson> lessonsGroupedByModule = await _readDbContext.Lessons
            .Where(l => moduleIds.Contains(l.ModuleId))
            .OrderBy(l => l.Index)
            .ToListAsync(cancellationToken);

        var lessonsByModuleId = lessonsGroupedByModule
            .GroupBy(l => l.ModuleId)
            .ToDictionary(g => g.Key, g => g.ToList());

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

            var courseContext = new CourseContext(course.Id, course.InstructorId, course.Status);
            var moduleContext = new ModuleContext(courseContext, module.Id);

            return new ModuleDto
            {
                Id = module.Id.Value,
                Title = module.Title.Value,
                Index = module.Index,
                LessonCount = lessonDtos.Count,
                Duration = TimeSpan.FromTicks(lessonDtos.Sum(l => l.Duration.Ticks)),
                LessonIds = lessonDtos.Select(l => l.LessonId).ToList(),
                Links = _linkBuilder.BuildLinks(LinkResourceKey.Module, moduleContext).ToList()
            };
        }).ToList();

        return Result.Success<IReadOnlyList<ModuleDto>>(moduleDetailsDtos);
    }
}

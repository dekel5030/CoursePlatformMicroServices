using Courses.Application.Abstractions.Data;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Modules.Queries.GetByCourseId;

internal sealed class GetModulesByCourseIdQueryHandler
    : IQueryHandler<GetModulesByCourseIdQuery, ModuleCollectionDto>
{
    private readonly IReadDbContext _readDbContext;

    public GetModulesByCourseIdQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<ModuleCollectionDto>> Handle(
        GetModulesByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        List<Module> modules = await _readDbContext.Modules
            .Where(module => module.CourseId.Value == request.CourseId.Value)
            .OrderBy(module => module.Index)
            .ToListAsync(cancellationToken);

        if (modules.Count == 0)
        {
            return Result.Success(new ModuleCollectionDto
            {
                Items = [],
                PageNumber = 1,
                PageSize = 1,
                TotalItems = 0,
                Links = null
            });
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

            return new ModuleDetailsDto(
                module.Id.Value,
                module.Title.Value,
                module.Index,
                moduleLessons.Count,
                TimeSpan.FromSeconds(moduleLessons.Sum(l => l.Duration.TotalSeconds)),
                lessonDtos
            );
        }).ToList();

        return Result.Success(new ModuleCollectionDto
        {
            Items = moduleDetailsDtos,
            PageNumber = 1,
            PageSize = moduleDetailsDtos.Count,
            TotalItems = moduleDetailsDtos.Count,
            Links = null
        });
    }
}

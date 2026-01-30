using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Modules.Dtos;
using Courses.Application.Shared.Extensions;
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
        List<ModuleReadModel> modules = await _readDbContext.Modules
            .Where(module => module.CourseId == request.CourseId.Value)
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

        List<LessonReadModel> lessons = await _readDbContext.Lessons
            .Where(lesson => moduleIds.Contains(lesson.ModuleId))
            .OrderBy(lesson => lesson.Index)
            .ToListAsync(cancellationToken);

        var lessonsByModule = lessons
            .GroupBy(l => l.ModuleId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var moduleDetailsDtos = modules.Select(module =>
        {
            lessonsByModule.TryGetValue(module.Id, out List<LessonReadModel>? moduleLessons);
            return module.ToModuleDetailsDto(moduleLessons ?? []);
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

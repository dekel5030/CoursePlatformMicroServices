using Courses.Application.Abstractions.Data;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Domain.Module;
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
            .Include(m => m.Lessons)
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

        var moduleDetailsDtos = modules.Select(module =>
        {
            var lessonDtos = module.Lessons
                .OrderBy(l => l.Index)
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
                module.Lessons.Count,
                TimeSpan.FromSeconds(module.Lessons.Sum(l => l.Duration.TotalSeconds)),
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

using System.Data;
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
            .Include(module => module.Lessons)
            .Where(module => module.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        var moduleDetailsDtos = modules.Select(module => new ModuleDetailsDto(
            module.Id.Value,
            module.Title.Value,
            module.Index,
            module.Lessons.Count,
            module.Duration,
            module.Lessons
                .OrderBy(lesson => lesson.Index)
                .Select(lesson => new LessonSummaryDto(
                    module.Id.Value,
                    lesson.Id.Value,
                    lesson.Title.Value,
                    lesson.Index,
                    lesson.Duration,
                    lesson.ThumbnailImageUrl?.Path,
                    lesson.Access)).ToList())).ToList();

        var moduleCollectionDto = new ModuleCollectionDto
        {
            Items = moduleDetailsDtos,
            PageNumber = 1,
            PageSize = 1,
            TotalItems = 1,
            Links = null
        };

        return Result.Success(moduleCollectionDto);
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Application.Modules.Dtos;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Modules.Queries.GetModules;

internal sealed class GetModulesQueryHandler
    : IQueryHandler<GetModulesQuery, IReadOnlyList<ModuleDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetModulesQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<ModuleDto>>> Handle(
        GetModulesQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Module> query = _readDbContext.Modules;

        query = ApplyFilters(request, query);

        List<Module> modules = await query
            .OrderBy(module => module.Index)
            .ToListAsync(cancellationToken);

        if (modules.Count == 0)
        {
            return Result.Success<IReadOnlyList<ModuleDto>>([]);
        }

        var moduleDtos = modules.Select(module => MapToDto(module)).ToList();

        return Result.Success<IReadOnlyList<ModuleDto>>(moduleDtos);
    }

    private static ModuleDto MapToDto(Module m)
    {
        return new ModuleDto
        {
            Id = m.Id.Value,
            Title = m.Title.Value,
            Links = []
        };
    }

    private static IQueryable<Module> ApplyFilters(GetModulesQuery request, IQueryable<Module> query)
    {
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

        return query;
    }
}

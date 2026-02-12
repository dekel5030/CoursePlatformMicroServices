using Courses.Application.Abstractions.Data;
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
            return Result.Success<IReadOnlyList<ModuleDto>>([]);
        }

        var moduleDtos = modules.Select(m => new ModuleDto
        {
            Id = m.Id.Value,
            Title = m.Title.Value,
            Links = []
        }).ToList();

        return Result.Success<IReadOnlyList<ModuleDto>>(moduleDtos);
    }
}

using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Write.Repositories;

internal sealed class ModulesRepository : RepositoryBase<Module, ModuleId>, IModuleRepository
{
    private readonly WriteDbContext _dbContext;

    public ModulesRepository(WriteDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Module?> GetByIdAsync(ModuleId id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Modules
            .FirstOrDefaultAsync(module => module.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Module>> GetAllByCourseIdAsync(
        CourseId courseId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Modules
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);
    }
}

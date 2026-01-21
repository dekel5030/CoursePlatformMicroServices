using Courses.Domain.Module;
using Courses.Domain.Module.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Repositories;

internal sealed class ModulesRepository : RepositoryBase<Module, ModuleId>, IModulesRepository
{
    private readonly WriteDbContext _dbContext;

    public ModulesRepository(WriteDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Module?> GetByIdAsync(ModuleId id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Modules.Include(module => module.Lessons)
            .FirstOrDefaultAsync(module => module.Id == id, cancellationToken);
    }
}

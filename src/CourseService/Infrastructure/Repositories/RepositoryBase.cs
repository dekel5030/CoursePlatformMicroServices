using Courses.Domain.Shared;
using Courses.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : IEquatable<TId>
{
    protected readonly AppDbContextBase _dbContext;

    protected RepositoryBase(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);
    }
    public virtual void Add(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
    }
}

using System.Linq.Expressions;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Shared;
using Courses.Infrastructure.Database.Write;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Write.Repositories;

public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IHasId<TId>
    where TId : IEquatable<TId>
{
    protected AppDbContextBase DbContext { get; }

    protected RepositoryBase(WriteDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);
    }

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public virtual void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }
}

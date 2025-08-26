namespace CourseService.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly CourseDbContext _dbContext;

    public UnitOfWork(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }
}

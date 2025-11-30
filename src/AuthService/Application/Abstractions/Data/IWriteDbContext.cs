namespace Application.Abstractions.Data;

public interface IWriteDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

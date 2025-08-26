namespace AuthService.Data;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync();
}
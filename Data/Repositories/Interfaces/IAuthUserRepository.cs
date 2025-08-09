using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IAuthUserRepository
{
    Task AddAuthUserAsync(AuthUser authUser);
    Task<AuthUser?> GetAuthUserByEmailAsync(string email, bool includeAccessData = false);
    void DeleteAuthUserAsync(AuthUser authUser);
    Task SaveChangesAsync();
}
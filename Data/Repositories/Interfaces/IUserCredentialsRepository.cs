using AuthService.Models;
using Common;

namespace AuthService.Data.Repositories.Interfaces;

public interface IUserCredentialsRepository
{
    Task AddUserCredentialsAsync(UserCredentials credentials);
    Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email, bool includeAccessData = false);
    void DeleteUserCredentialsAsync(UserCredentials credentials);
    Task SaveChangesAsync();
}
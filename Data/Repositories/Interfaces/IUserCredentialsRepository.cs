using AuthService.Models;
using Common;

namespace AuthService.Data.Repositories.Interfaces;

public interface IUserCredentialsRepository
{
    Task AddUserCredentialsAsync(UserCredentials credentials);
    Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email);
    Task<Result<UserCredentials>> DeleteUserCredentialsAsync(int id);
    Task SaveChangesAsync();
}
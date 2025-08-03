using AuthService.Models;
using Common;

namespace AuthService.Data;

public interface IAuthRepository
{
    Task AddUserCredentialsAsync(UserCredentials credentials);
    Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email);
    Task<Result<UserCredentials>> DeleteUserCredentialsAsync(int id);
    Task SaveChangesAsync();
}
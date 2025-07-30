using AuthService.Models;

namespace AuthService.Data;

public interface IAuthRepository
{
    Task AddUserCredentialsAsync(UserCredentials credentials);
    Task<UserCredentials?> GetByEmailAsync(string email);
    Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email);
    Task SaveChangesAsync();
}
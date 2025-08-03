using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _dbContext;

    public AuthRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserCredentialsAsync(UserCredentials credentials)
    {
        await _dbContext.UserCredentials.AddAsync(credentials);
    }
    
    public async Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email)
    {
        return await _dbContext.UserCredentials
            .FirstOrDefaultAsync(uc => uc.Email == email.ToLower());
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
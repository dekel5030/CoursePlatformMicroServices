using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class UserCredentialsRepository : IUserCredentialsRepository
{
    private readonly AuthDbContext _dbContext;

    public UserCredentialsRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserCredentialsAsync(UserCredentials credentials)
    {
        await _dbContext.UserCredentials.AddAsync(credentials);
    }

    public Task<Result<UserCredentials>> DeleteUserCredentialsAsync(int id)
    {
        throw new NotImplementedException();
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
using EnrollmentService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly EnrollmentDbContext _dbContext;

    public UserRepository(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> UserExistsAsync(int userId, CancellationToken ct = default)
    {
        return _dbContext.KnownUsers.AnyAsync(u => u.UserId == userId, ct);
    }
}

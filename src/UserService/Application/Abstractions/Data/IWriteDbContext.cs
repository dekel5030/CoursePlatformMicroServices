using Microsoft.EntityFrameworkCore;
using Users.Domain.Users;

namespace Users.Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Users.Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
using Domain.AuthUsers;
using Domain.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<AuthUser> AuthUsers { get; }
    DbSet<Permission> Permissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

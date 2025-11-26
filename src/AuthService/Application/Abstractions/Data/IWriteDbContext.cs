using Domain.AuthUsers;
using Domain.Permissions;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<AuthUser> AuthUsers { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

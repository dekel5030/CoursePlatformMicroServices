using Domain.AuthUsers;
using Domain.Permissions;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<AuthUser> AuthUsers { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
}

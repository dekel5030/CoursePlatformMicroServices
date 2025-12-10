using Domain.AuthUsers;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<Role> Roles { get; }
    DbSet<AuthUser> AuthUsers { get; }
}

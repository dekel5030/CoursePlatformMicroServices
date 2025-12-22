using Auth.Domain.AuthUsers;
using Auth.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<Role> Roles { get; }
    DbSet<AuthUser> Users { get; }
}

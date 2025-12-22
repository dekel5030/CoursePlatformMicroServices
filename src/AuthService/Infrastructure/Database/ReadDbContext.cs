using Auth.Application.Abstractions.Data;
using Auth.Domain.AuthUsers;
using Auth.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Database;

public class ReadDbContext : AppDbContextBase, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    IQueryable<AuthUser> IReadDbContext.Users => Users.AsNoTracking().AsQueryable();

    IQueryable<Role> IReadDbContext.Roles => Roles.AsNoTracking().AsQueryable();
}
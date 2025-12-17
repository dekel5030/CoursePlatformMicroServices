using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ReadDbContext : AppDbContextBase, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    IQueryable<AuthUser> IReadDbContext.Users => Users.AsNoTracking().AsQueryable();

    IQueryable<Role> IReadDbContext.Roles => Roles.AsNoTracking().AsQueryable();
}
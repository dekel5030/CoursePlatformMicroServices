using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    public DbSet<AuthUser> Users { get; set; } 
    public DbSet<Role> Roles { get; set; }

    IQueryable<AuthUser> IReadDbContext.AuthUsers => Users.AsNoTracking().AsQueryable();

    IQueryable<Role> IReadDbContext.Roles => Roles.AsNoTracking().AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}

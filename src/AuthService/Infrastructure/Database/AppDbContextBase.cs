using Auth.Domain.AuthUsers;
using Auth.Domain.Roles;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Database;

public abstract class AppDbContextBase : DbContext
{
    public DbSet<AuthUser> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public AppDbContextBase(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
        modelBuilder.AddTransactionalOutboxEntities();
    }
}
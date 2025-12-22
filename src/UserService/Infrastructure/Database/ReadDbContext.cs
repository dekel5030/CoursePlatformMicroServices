using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;

namespace Users.Infrastructure.Database;

public class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}
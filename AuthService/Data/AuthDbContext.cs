using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    public DbSet<UserCredentials> UserCredentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCredentials>()
            .HasIndex(u => u.UserId)
            .IsUnique();

        modelBuilder.Entity<UserCredentials>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("NOW()") 
            .ValueGeneratedOnAdd()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<UserCredentials>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        base.OnModelCreating(modelBuilder);
    }
}
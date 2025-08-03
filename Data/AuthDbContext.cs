using AuthService.Models;
using Microsoft.EntityFrameworkCore;

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
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UserCredentials>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<UserCredentials>()
            .Property(u => u.Role)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeEmails();
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void NormalizeEmails()
    {
        foreach (var entry in ChangeTracker.Entries<UserCredentials>())
        {
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified) &&
                !string.IsNullOrWhiteSpace(entry.Entity.Email))
            {
                entry.Entity.Email = entry.Entity.Email.ToLowerInvariant();
            }
        }
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<UserCredentials>())
        {
            if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.IsModified))
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
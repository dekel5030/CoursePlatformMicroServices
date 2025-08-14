using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Context;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    public DbSet<AuthUser> AuthUser { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureAuthUser(modelBuilder);
        ConfigurePermission(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureRolePermission(modelBuilder);
        ConfigureUserRole(modelBuilder);
        ConfigureUserPermission(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeEmails();
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ConfigureAuthUser(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<AuthUser>();

        user.HasKey(u => u.Id);

        user.HasIndex(u => u.UserId).IsUnique();
        user.HasIndex(u => u.Email).IsUnique();

        user.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        user.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        user.HasMany(u => u.UserPermissions)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId);
    }

    private void ConfigurePermission(ModelBuilder modelBuilder)
    {
        var permission = modelBuilder.Entity<Permission>();

        permission.HasKey(p => p.Id);
        permission.HasIndex(p => p.Name).IsUnique();
    }

    private void ConfigureRole(ModelBuilder modelBuilder)
    {
        var role = modelBuilder.Entity<Role>();

        role.HasKey(r => r.Id);
        role.HasIndex(r => r.Name).IsUnique();

        role.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId);
    }

    private void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        var rp = modelBuilder.Entity<RolePermission>();

        rp.HasKey(x => new { x.RoleId, x.PermissionId });

        rp.HasOne(x => x.Role)
        .WithMany(r => r.RolePermissions)
        .HasForeignKey(x => x.RoleId);

        rp.HasOne(x => x.Permission)
        .WithMany()
        .HasForeignKey(x => x.PermissionId);
    }

    private void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        var ur = modelBuilder.Entity<UserRole>();

        ur.HasKey(x => new { x.UserId, x.RoleId });

        ur.HasOne(x => x.User)
        .WithMany(u => u.UserRoles)
        .HasForeignKey(x => x.UserId);

        ur.HasOne(x => x.Role)
        .WithMany()
        .HasForeignKey(x => x.RoleId);
    }

    private void ConfigureUserPermission(ModelBuilder modelBuilder)
    {
        var up = modelBuilder.Entity<UserPermission>();

        up.HasKey(x => new { x.UserId, x.PermissionId });

        up.HasOne(x => x.User)
        .WithMany(u => u.UserPermissions)
        .HasForeignKey(x => x.UserId);

        up.HasOne(x => x.Permission)
        .WithMany()
        .HasForeignKey(x => x.PermissionId);
    }

    private void NormalizeEmails()
    {
        foreach (var entry in ChangeTracker.Entries<AuthUser>())
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
        foreach (var entry in ChangeTracker.Entries<AuthUser>())
        {
            if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.IsModified))
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
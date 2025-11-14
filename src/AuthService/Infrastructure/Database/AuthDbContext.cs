using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Domain.Roles;
using Domain.Roles.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AuthDbContext : DbContext, IWriteDbContext, IReadDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<AuthUser> AuthUsers { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

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

        user.ToTable("AuthUsers");

        user.HasKey(u => u.Id);

        user.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new AuthUserId(value));

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

        user.Navigation(u => u.UserRoles).AutoInclude();
        user.Navigation(u => u.UserPermissions).AutoInclude();
    }

    private void ConfigurePermission(ModelBuilder modelBuilder)
    {
        var permission = modelBuilder.Entity<Permission>();

        permission.ToTable("Permissions");

        permission.HasKey(p => p.Id);

        permission.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PermissionId(value));

        permission.HasIndex(p => p.Name).IsUnique();
    }

    private void ConfigureRole(ModelBuilder modelBuilder)
    {
        var role = modelBuilder.Entity<Role>();

        role.ToTable("Roles");

        role.HasKey(r => r.Id);

        role.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value));

        role.HasIndex(r => r.Name).IsUnique();

        role.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId);

        role.Navigation(r => r.RolePermissions).AutoInclude();
    }

    private void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        var rp = modelBuilder.Entity<RolePermission>();

        rp.ToTable("RolePermissions");

        rp.HasKey(x => new { x.RoleId, x.PermissionId });

        rp.Property(x => x.RoleId)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value));

        rp.Property(x => x.PermissionId)
            .HasConversion(
                id => id.Value,
                value => new PermissionId(value));

        rp.HasOne(x => x.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(x => x.RoleId);

        rp.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId);

        rp.Navigation(x => x.Permission).AutoInclude();
    }

    private void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        var ur = modelBuilder.Entity<UserRole>();

        ur.ToTable("UserRoles");

        ur.HasKey(x => new { x.UserId, x.RoleId });

        ur.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => new AuthUserId(value));

        ur.Property(x => x.RoleId)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value));

        ur.HasOne(x => x.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(x => x.UserId);

        ur.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        ur.Navigation(x => x.Role).AutoInclude();
    }

    private void ConfigureUserPermission(ModelBuilder modelBuilder)
    {
        var up = modelBuilder.Entity<UserPermission>();

        up.ToTable("UserPermissions");

        up.HasKey(x => new { x.UserId, x.PermissionId });

        up.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => new AuthUserId(value));

        up.Property(x => x.PermissionId)
            .HasConversion(
                id => id.Value,
                value => new PermissionId(value));

        up.HasOne(x => x.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(x => x.UserId);

        up.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId);

        up.Navigation(x => x.Permission).AutoInclude();
    }

    private void NormalizeEmails()
    {
        foreach (var entry in ChangeTracker.Entries<AuthUser>())
        {
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified) &&
                !string.IsNullOrWhiteSpace(entry.Entity.Email))
            {
                entry.Property(nameof(AuthUser.Email)).CurrentValue = 
                    entry.Entity.Email.ToLowerInvariant();
            }
        }
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<AuthUser>())
        {
            if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.IsModified))
            {
                entry.Property(nameof(AuthUser.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}

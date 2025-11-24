using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> user)
    {
        user.ToTable("AuthUsers");

        user.HasKey(u => u.Id);

        user.Property(u => u.Id)
            .HasConversion(id => id.Value, value => new AuthUserId(value));

        user.HasIndex(u => u.Email).IsUnique();

        user.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        user.Property(u => u.RefreshToken)
            .HasMaxLength(500)
            .IsRequired(false);

        user.Property(u => u.RefreshTokenExpiresAt)
            .IsRequired(false);

        user.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        user.HasMany(u => u.UserPermissions)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId);

        user.Navigation(u => u.UserRoles).AutoInclude();
        user.Navigation(u => u.UserPermissions).AutoInclude();
    }
}

using Domain.AuthUsers;
using Domain.Permissions;
using Domain.Roles;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> user)
    {
        user.ToTable("domain_users");

        user.Property(u => u.Id).ValueGeneratedNever();

        user
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "domain_user_roles",
                r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                u => u.HasOne<AuthUser>().WithMany().HasForeignKey("UserId")
            );

        user.HasMany(u => u.Permissions)
            .WithOne()
            .HasForeignKey("AuthUserId")
            .OnDelete(DeleteBehavior.Cascade);

        user.Ignore(u => u.DomainEvents);
    }
}
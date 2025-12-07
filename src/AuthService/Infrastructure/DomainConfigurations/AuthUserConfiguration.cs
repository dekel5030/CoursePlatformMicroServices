using Domain.AuthUsers;
using Domain.Permissions;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> user)
    {
        user.ToTable("auth_users");
        user.Property(u => u.Id).ValueGeneratedNever();
        user.Ignore(u => u.DomainEvents);
        //user.Ignore(u => u.Roles);

        user.OwnsMany(u => u.Permissions, permissionBuilder =>
        {
            permissionBuilder.ToTable("user_permissions");

            permissionBuilder.WithOwner().HasForeignKey("UserId");
            permissionBuilder.HasKey(
                "UserId",
                nameof(UserPermission.Resource),
                nameof(UserPermission.Action),
                nameof(UserPermission.ResourceId));

            permissionBuilder.Property(p => p.Effect)
                .HasConversion<string>();

            permissionBuilder.Property(p => p.Resource)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            permissionBuilder.Property(p => p.Action)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            permissionBuilder.Property(p => p.ResourceId)
                .HasColumnName("resource_id")
                .HasMaxLength(100)
                .IsRequired()
                .HasConversion(
                    resourceId => resourceId.Value,
                    value => ResourceId.Create(value)
                );
        });

    }
}
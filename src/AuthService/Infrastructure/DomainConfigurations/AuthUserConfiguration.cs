using Domain.AuthUsers;
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


        user.OwnsMany(u => u.Permissions, permissionBuilder =>
        {
            permissionBuilder.ToJson();
            permissionBuilder.Property(p => p.Effect).HasMaxLength(50);
            permissionBuilder.Property(p => p.Action).HasMaxLength(100);
            permissionBuilder.Property(p => p.Resource).HasMaxLength(100);

            permissionBuilder.Property(p => p.ResourceId)
                .HasConversion(id => id.Value, val => ResourceId.Create(val));
        });

        user.Ignore(u => u.DomainEvents);
    }
}
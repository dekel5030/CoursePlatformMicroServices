using Domain.Permissions;
using Domain.Roles;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("domain_roles");

        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Ignore(r => r.DomainEvents);

        builder.OwnsMany(r => r.Permissions, permissionBuilder =>
        {
            permissionBuilder.ToTable("role_permissions");

            permissionBuilder.WithOwner().HasForeignKey("RoleId");

            permissionBuilder.HasKey(
                "RoleId", 
                nameof(RolePermission.Resource), 
                nameof(RolePermission.Action), 
                nameof(RolePermission.ResourceId));

            permissionBuilder.Ignore(p => p.Effect);

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
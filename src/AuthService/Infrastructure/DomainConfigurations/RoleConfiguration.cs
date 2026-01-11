using Auth.Domain.Roles;
using Auth.Domain.Roles.Primitives;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.DomainConfigurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role> 
{ 
    public void Configure(EntityTypeBuilder<Role> builder) 
    { 
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value))
            .ValueGeneratedNever();

        builder.Property(r => r.Name)
            .HasConversion(
                name => name.Value,
                value => new RoleName(value));

        builder.OwnsMany(r => r.Permissions, permissionBuilder => 
        { 
            permissionBuilder.ToJson("permissions");
            permissionBuilder.Property(p => p.Effect).HasConversion<string>();
            permissionBuilder.Property(p => p.Action).HasConversion<string>();
            permissionBuilder.Property(p => p.Resource).HasConversion<string>();
            permissionBuilder.Property(p => p.ResourceId)
                .HasConversion(id => id.Value, val => ResourceId.Create(val));
        });

        builder.Ignore(r => r.DomainEvents);
    } 
}

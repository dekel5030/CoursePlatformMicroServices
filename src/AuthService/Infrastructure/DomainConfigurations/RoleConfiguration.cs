using Domain.Roles;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role> 
{ 
    public void Configure(EntityTypeBuilder<Role> builder) 
    { 
        builder.ToTable("domain_roles");

        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.OwnsMany(r => r.Permissions, permissionBuilder => 
        { 
            permissionBuilder.ToJson();
            permissionBuilder.Property(p => p.Action).HasMaxLength(100);
            permissionBuilder.Property(p => p.Resource).HasMaxLength(100);
            permissionBuilder.Property(p => p.ResourceId)
                .HasConversion(id => id.Value, val => ResourceId.Create(val));
        });

        builder.Ignore(r => r.DomainEvents);
    } 
}
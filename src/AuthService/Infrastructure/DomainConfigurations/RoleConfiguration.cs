using Domain.Roles;
using Domain.Roles.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> role)
    {
        role.ToTable("Roles");

        role.HasKey(r => r.Id);

        role.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new RoleId(value));

        role.HasIndex(r => r.Name).IsUnique();

        role.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId);

        role.Navigation(r => r.RolePermissions).AutoInclude();
    }
}

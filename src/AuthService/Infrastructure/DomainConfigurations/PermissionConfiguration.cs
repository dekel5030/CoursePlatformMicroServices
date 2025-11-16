using Domain.Permissions.Primitives;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Auth;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> permission)
    {
        permission.ToTable("Permissions");

        permission.HasKey(p => p.Id);

        permission.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PermissionId(value));

        permission.HasIndex(p => p.Name).IsUnique();
    }
}

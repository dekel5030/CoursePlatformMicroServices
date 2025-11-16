using Domain.Permissions.Primitives;
using Domain.Roles;
using Domain.Roles.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Auth;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> rp)
    {
        rp.ToTable("RolePermissions");

        rp.HasKey(x => new { x.RoleId, x.PermissionId });

        rp.Property(x => x.RoleId)
            .HasConversion(id => id.Value, value => new RoleId(value));

        rp.Property(x => x.PermissionId)
            .HasConversion(id => id.Value, value => new PermissionId(value));

        rp.HasOne(x => x.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(x => x.RoleId);

        rp.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId);

        rp.Navigation(x => x.Permission).AutoInclude();
    }
}

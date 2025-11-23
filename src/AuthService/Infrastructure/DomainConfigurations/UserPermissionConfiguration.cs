using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> up)
    {
        up.ToTable("UserPermissions");

        up.HasKey(x => new { x.UserId, x.PermissionId });

        up.Property(x => x.UserId)
            .HasConversion(id => id.Value, value => new AuthUserId(value));

        up.Property(x => x.PermissionId)
            .HasConversion(id => id.Value, value => new PermissionId(value));

        up.HasOne(x => x.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(x => x.UserId);

        up.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId);

        up.Navigation(x => x.Permission).AutoInclude();
    }
}

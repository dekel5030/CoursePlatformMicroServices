using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Domain.Roles.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> ur)
    {
        ur.ToTable("UserRoles");

        ur.HasKey(x => new { x.UserId, x.RoleId });

        ur.Property(x => x.UserId)
            .HasConversion(id => id.Value, value => new AuthUserId(value));

        ur.Property(x => x.RoleId)
            .HasConversion(id => id.Value, value => new RoleId(value));

        ur.HasOne(x => x.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(x => x.UserId);

        ur.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        ur.Navigation(x => x.Role).AutoInclude();
    }
}

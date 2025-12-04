using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Users;

public class ApplicationIdentityRoleConfiguration : IEntityTypeConfiguration<ApplicationIdentityRole>
{
    public void Configure(EntityTypeBuilder<ApplicationIdentityRole> builder)
    {
        builder.ToTable(RoleTableNames.IdentityRoles);

        builder.HasOne(r => r.DomainRole)
               .WithOne()
               .HasForeignKey<Role>(r => r.Id)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RolerConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(RoleTableNames.IdentityRoles);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
               .HasColumnName(nameof(ApplicationIdentityRole.Name)) 
               .IsRequired();
    }
}

public static class RoleTableNames
{
    public const string IdentityRoles = "AspNetRoles";
}
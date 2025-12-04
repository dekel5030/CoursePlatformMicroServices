using Domain.AuthUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Users;

public class ApplicationIdentityUserConfiguration : IEntityTypeConfiguration<ApplicationIdentityUser>
{
    public void Configure(EntityTypeBuilder<ApplicationIdentityUser> builder)
    {
        builder.ToTable(TableNames.IdentityUsers);

        builder.HasOne(u => u.DomainUser)
               .WithOne()
               .HasForeignKey<AuthUser>(u => u.Id)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable(TableNames.IdentityUsers);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
               .HasColumnName(nameof(ApplicationIdentityUser.Email))
               .IsRequired();

        builder.Property(u => u.UserName)
               .HasColumnName(nameof(ApplicationIdentityUser.UserName)) 
               .IsRequired();
    }
}

public static class TableNames
{
    public const string IdentityUsers = "AspNetUsers";
}
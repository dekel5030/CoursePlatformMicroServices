using Domain.AuthUsers;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> user)
    {
        user.ToTable("auth_users");

        user.Ignore(u => u.DomainEvents);
        user.Ignore(u => u.Roles);

        user.Property(u => u.Id).ValueGeneratedNever();
    }
}

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("domain_roles");
        
        builder.Ignore(r => r.DomainEvents);

        builder.Property(r => r.Id).ValueGeneratedNever();
    }
}
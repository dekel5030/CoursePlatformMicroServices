using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Primitives;
using Auth.Domain.Roles;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.DomainConfigurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);
        builder.HasIndex(user => user.IdentityId).IsUnique();
        builder.HasIndex(user => user.Email).IsUnique();
        builder.Property(user => user.Id).ValueGeneratedNever();

        builder.Property(user => user.Id).HasConversion(
            id => id.Value,
            value => new AuthUserId(value));

        builder.Property(user => user.IdentityId).HasConversion(
            id => id.ProviderId,
            value => new IdentityProviderId(value));

        builder.Property(user => user.Email).HasConversion(
            email => email.Address,
            value => new Email(value));

        builder.OwnsOne(user => user.FullName, nameBuilder =>
        {
            nameBuilder.Property(name => name.FirstName)
                .HasColumnName("first_name")
                .HasConversion(
                    firstName => firstName.Name,
                    value => new FirstName(value));

            nameBuilder.Property(name => name.LastName)
                .HasColumnName("last_name")
                .HasConversion(
                    lastName => lastName.Name,
                    value => new LastName(value));
        });

        builder
            .HasMany(user => user.Roles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                r => r.HasOne<Role>().WithMany().HasForeignKey("role_id"),
                u => u.HasOne<AuthUser>().WithMany().HasForeignKey("user_id")
            );


        builder.OwnsMany(u => u.Permissions, permissionBuilder =>
        {
            permissionBuilder.ToJson("permissions");
            permissionBuilder.Property(p => p.Effect).HasConversion<string>();
            permissionBuilder.Property(p => p.Action).HasConversion<string>();
            permissionBuilder.Property(p => p.Resource).HasConversion<string>();

            permissionBuilder.Property(p => p.ResourceId)
                .HasConversion(id => id.Value, val => ResourceId.Create(val));
        });

        builder.Ignore(u => u.DomainEvents);
    }
}
using Domain.Users;
using Domain.Users.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class KnownUserConfiguration : IEntityTypeConfiguration<KnownUser>
{
    public void Configure(EntityTypeBuilder<KnownUser> builder)
    {
        builder.ToTable("known_users");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasConversion(
                id => id.Value,
                value => new ExternalUserId(value))
            .HasColumnName("user_id");

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
    }
}

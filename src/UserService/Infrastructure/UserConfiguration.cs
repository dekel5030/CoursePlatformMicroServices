using Domain.Users;
using Domain.Users.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const int _emailMaxLength = 256;
    private const int _authUserIdMaxLength = 50;
    private const string _tableName = "users";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(_tableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(u => u.AuthUserId)
            .HasConversion(
                authUserId => authUserId.Value,
                value => new AuthUserId(value))
            .HasMaxLength(_authUserIdMaxLength)
            .IsRequired(false); 
        
        builder.HasIndex(u => u.AuthUserId).IsUnique();

        builder.Property(u => u.Email).IsRequired().HasMaxLength(_emailMaxLength);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.OwnsOne(user => user.FullName);
        builder.OwnsOne(user => user.PhoneNumber);

        builder.Property(u => u.DateOfBirth)
            .HasColumnType("timestamp without time zone");
    }
}

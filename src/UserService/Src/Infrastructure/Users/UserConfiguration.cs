using Domain.Users;
using Domain.Users.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const int _emailMaxLength = 256;
    private const string _tableName = "users";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(_tableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(u => u.Email).IsRequired().HasMaxLength(_emailMaxLength);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.ComplexProperty(user => user.FullName);
        builder.ComplexProperty(user => user.PhoneNumber);
    }
}

using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class InstructorReadModelConfiguration : IEntityTypeConfiguration<InstructorReadModel>
{
    public void Configure(EntityTypeBuilder<InstructorReadModel> builder)
    {
        builder.ToTable("instructors", SchemaNames.Read);
        builder.HasKey(i => i.Id);

        // Identity
        builder.Property(i => i.FirstName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.LastName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(320);
        builder.Property(i => i.FullName).IsRequired().HasMaxLength(410);

        // Media
        builder.Property(i => i.AvatarUrl).HasMaxLength(1000);

        // Timestamps
        builder.Property(i => i.CreatedAtUtc).IsRequired();
        builder.Property(i => i.UpdatedAtUtc).IsRequired();

        // Indexes
        builder.HasIndex(i => i.Email);
    }
}

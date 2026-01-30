using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CourseReadModelConfiguration : IEntityTypeConfiguration<CourseReadModel>
{
    public void Configure(EntityTypeBuilder<CourseReadModel> builder)
    {
        builder.ToTable("courses", SchemaNames.Read);
        builder.HasKey(c => c.Id);

        // Core Metadata
        builder.Property(c => c.Title).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Description).IsRequired().HasMaxLength(5000);
        builder.Property(c => c.Slug).HasMaxLength(550);
        builder.Property(c => c.Status).IsRequired();
        builder.Property(c => c.Difficulty).IsRequired();
        builder.Property(c => c.Language).IsRequired().HasMaxLength(50);

        // Pricing
        builder.Property(c => c.PriceAmount).HasPrecision(18, 2);
        builder.Property(c => c.PriceCurrency).IsRequired().HasMaxLength(3);

        // Foreign Keys
        builder.Property(c => c.InstructorId).IsRequired();
        builder.Property(c => c.CategoryId).IsRequired();

        // Collections stored as JSON
        builder.Property(c => c.ImageUrls)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb");

        builder.Property(c => c.Tags)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb");

        // Stats
        builder.Property(c => c.TotalModules).HasDefaultValue(0);
        builder.Property(c => c.TotalLessons).HasDefaultValue(0);
        builder.Property(c => c.TotalDurationSeconds).HasDefaultValue(0);
        builder.Property(c => c.EnrollmentCount).HasDefaultValue(0);

        // Timestamps
        builder.Property(c => c.CreatedAtUtc).IsRequired();
        builder.Property(c => c.UpdatedAtUtc).IsRequired();

        // Indexes for common queries
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.InstructorId);
        builder.HasIndex(c => c.CategoryId);
        builder.HasIndex(c => c.UpdatedAtUtc);
    }
}

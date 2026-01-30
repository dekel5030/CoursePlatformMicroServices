using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class LessonReadModelConfiguration : IEntityTypeConfiguration<LessonReadModel>
{
    public void Configure(EntityTypeBuilder<LessonReadModel> builder)
    {
        builder.ToTable("lessons", SchemaNames.Read);
        builder.HasKey(l => l.Id);

        // Core Metadata
        builder.Property(l => l.Title).IsRequired().HasMaxLength(500);
        builder.Property(l => l.Description).IsRequired().HasMaxLength(5000);
        builder.Property(l => l.Slug).HasMaxLength(550);
        builder.Property(l => l.Index).IsRequired();

        // Foreign Keys
        builder.Property(l => l.ModuleId).IsRequired();
        builder.Property(l => l.CourseId).IsRequired();

        // Access Control
        builder.Property(l => l.Access).IsRequired();

        // Media
        builder.Property(l => l.VideoUrl).HasMaxLength(1000);
        builder.Property(l => l.ThumbnailUrl).HasMaxLength(1000);
        builder.Property(l => l.TranscriptUrl).HasMaxLength(1000);
        builder.Property(l => l.Duration).IsRequired();

        // Timestamps
        builder.Property(l => l.CreatedAtUtc).IsRequired();
        builder.Property(l => l.UpdatedAtUtc).IsRequired();

        // Indexes for composition queries
        builder.HasIndex(l => l.ModuleId);
        builder.HasIndex(l => l.CourseId);
        builder.HasIndex(l => new { l.ModuleId, l.Index }); // For ordered queries
    }
}

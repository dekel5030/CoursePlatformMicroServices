using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class ModuleReadModelConfiguration : IEntityTypeConfiguration<ModuleReadModel>
{
    public void Configure(EntityTypeBuilder<ModuleReadModel> builder)
    {
        builder.ToTable("modules", SchemaNames.Read);
        builder.HasKey(m => m.Id);

        // Core Metadata
        builder.Property(m => m.Title).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Index).IsRequired();

        // Foreign Keys
        builder.Property(m => m.CourseId).IsRequired();

        // Stats
        builder.Property(m => m.LessonCount).HasDefaultValue(0);
        builder.Property(m => m.TotalDurationSeconds).HasDefaultValue(0);

        // Timestamps
        builder.Property(m => m.CreatedAtUtc).IsRequired();
        builder.Property(m => m.UpdatedAtUtc).IsRequired();

        // Indexes for composition queries
        builder.HasIndex(m => m.CourseId);
        builder.HasIndex(m => new { m.CourseId, m.Index }); // For ordered queries
    }
}

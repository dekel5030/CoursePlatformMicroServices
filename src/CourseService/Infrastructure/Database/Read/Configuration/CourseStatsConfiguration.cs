using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CourseStatsConfiguration : IEntityTypeConfiguration<CourseStatsReadModel>
{
    public void Configure(EntityTypeBuilder<CourseStatsReadModel> builder)
    {
        builder.ToTable("course_stats", SchemaNames.Read);
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CourseId).IsRequired();
        builder.HasIndex(s => s.CourseId).IsUnique();

        builder.Property(s => s.TotalDuration).IsRequired();
        builder.Property(s => s.LessonsCount).IsRequired();
        builder.Property(s => s.ModulesCount).IsRequired();
        builder.Property(s => s.EnrollmentCount).IsRequired();
    }
}

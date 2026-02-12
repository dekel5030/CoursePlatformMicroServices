using Courses.Application.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Write.Configuration;

public sealed class CourseAnalyticsConfiguration : IEntityTypeConfiguration<CourseAnalytics>
{
    public void Configure(EntityTypeBuilder<CourseAnalytics> builder)
    {
        builder.ToTable("CourseAnalytics", SchemaNames.Read);

        builder.HasKey(course => course.CourseId);

        builder.Property(c => c.TotalCourseDuration)
            .HasConversion(ticks => ticks.Ticks, value => TimeSpan.FromTicks(value));

        builder.OwnsMany(c => c.ModuleAnalytics, owned =>
        {
            owned.ToJson("module_analytics");
            owned.Property(m => m.TotalModuleDuration)
                .HasConversion(ticks => ticks.Ticks, value => TimeSpan.FromTicks(value));
        });
    }
}

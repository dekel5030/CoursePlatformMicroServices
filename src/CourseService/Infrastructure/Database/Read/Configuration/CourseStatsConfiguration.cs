using System.Text.Json;
using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CourseStatsConfiguration : IEntityTypeConfiguration<CourseStatsReadModel>
{
    public void Configure(EntityTypeBuilder<CourseStatsReadModel> builder)
    {
        builder.ToTable("course_stats", SchemaNames.Read);
        builder.HasKey(s => s.CourseId);

        builder.Property(s => s.TotalDuration).IsRequired();
        builder.Property(s => s.LessonsCount).IsRequired();
        builder.Property(s => s.ModulesCount).IsRequired();
        builder.Property(s => s.EnrollmentCount).IsRequired();

        var valueComparer = new ValueComparer<Dictionary<Guid, TimeSpan>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
            c => c.ToDictionary(entry => entry.Key, entry => entry.Value)
        );

        builder.Property(s => s.LessonDurations)
            .HasColumnType("jsonb")
            .HasColumnName("lesson_durations")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<Guid, TimeSpan>>(v, (JsonSerializerOptions?)null)
                     ?? new Dictionary<Guid, TimeSpan>())
            .Metadata.SetValueComparer(valueComparer);
    }
}

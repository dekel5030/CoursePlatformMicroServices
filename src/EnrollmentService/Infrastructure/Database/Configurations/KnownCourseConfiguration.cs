using Domain.Courses;
using Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class KnownCourseConfiguration : IEntityTypeConfiguration<KnownCourse>
{
    public void Configure(EntityTypeBuilder<KnownCourse> builder)
    {
        builder.ToTable("known_courses");

        builder.HasKey(c => c.CourseId);

        builder.Property(c => c.CourseId)
            .HasConversion(
                id => id.Value,
                value => new ExternalCourseId(value))
            .HasColumnName("course_id");

        builder.Property(c => c.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
    }
}

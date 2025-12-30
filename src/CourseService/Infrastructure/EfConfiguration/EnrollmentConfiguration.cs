using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.EfConfiguration;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(id => id.Value, v => new EnrollmentId(v));

        builder.Property(e => e.CourseId)
            .HasConversion(id => id.Value, v => new CourseId(v));

        builder.Property(e => e.StudentId)
            .HasConversion(id => id.Value, v => new StudentId(v));

        builder.HasIndex(e => new { e.CourseId, e.StudentId });

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Configuration;

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
            .HasConversion(id => id.Value, v => new UserId(v));

        builder.Property(e => e.Status)
            .HasConversion<string>();

        string statusColumnName = builder.Property(e => e.Status).Metadata.GetColumnName();
        string activeValue = EnrollmentStatus.Active.ToString();

        builder.HasIndex(e => new { e.CourseId, e.StudentId })
            .IsUnique()
            .HasFilter($"\"{statusColumnName}\" = '{activeValue}'");

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}

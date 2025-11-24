using Domain.Enrollments;
using Domain.Enrollments.Primitives;
using Domain.Users.Primitives;
using Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new EnrollmentId(value))
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId)
            .HasConversion(
                id => id.Value,
                value => new ExternalUserId(value))
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(e => e.CourseId)
            .HasConversion(
                id => id.Value,
                value => new ExternalCourseId(value))
            .HasColumnName("course_id")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.EnrolledAt)
            .HasColumnName("enrolled_at")
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(e => e.EntityVersion)
            .HasColumnName("entity_version")
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        builder.Ignore(e => e.DomainEvents);
    }
}

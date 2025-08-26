using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnrollmentService.Data.Configurations;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Enrollment> builder)
    {
        var enrollment = builder;

        enrollment.HasKey(e => e.Id);
        enrollment.HasIndex(e => new { e.CourseId, e.UserId }).IsUnique();

        enrollment.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>();

        enrollment.Property(x => x.EnrolledAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAdd();

        enrollment.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAddOrUpdate();

        enrollment.Property(x => x.ExpiresAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        enrollment.Property(x => x.EnrolledAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        enrollment.Property(x => x.EnrolledAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        enrollment.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        enrollment.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        enrollment
            .Property<uint>("xmin")
            .IsRowVersion()
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate();     
    }
}
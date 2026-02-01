using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings;
using Courses.Domain.Ratings.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Write.Configuration;

public sealed class CourseRatingConfiguration : IEntityTypeConfiguration<CourseRating>
{
    public void Configure(EntityTypeBuilder<CourseRating> builder)
    {
        builder.ToTable("CourseRatings");

        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.CourseId, r.UserId }).IsUnique();

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new RatingId(value));

        builder.Property(r => r.CourseId)
            .HasConversion(
                id => id.Value,
                value => new CourseId(value));

        builder.Property(r => r.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(r => r.Score)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(500);

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

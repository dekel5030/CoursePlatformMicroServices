using Courses.Domain.Courses.Primitives;
using Courses.Domain.CourseViews;
using Courses.Domain.CourseViews.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Write.Configuration;

public class CourseViewConfiguration : IEntityTypeConfiguration<CourseView>
{
    public void Configure(EntityTypeBuilder<CourseView> builder)
    {
        builder.ToTable("CourseViews");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, v => new CourseViewId(v));

        builder.Property(v => v.CourseId)
            .HasConversion(id => id.Value, v => new CourseId(v));

        builder.Property(v => v.UserId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                v => v.HasValue ? new UserId(v.Value) : null);

        builder.Property(v => v.ViewedAt)
            .IsRequired();

        builder.Ignore(v => v.DomainEvents);
    }
}

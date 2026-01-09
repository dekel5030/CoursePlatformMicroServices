using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.EfConfiguration;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(course => course.Id);

        builder.Property(course => course.Id)
            .HasConversion(
                id => id.Value,
                value => new CourseId(value));

        builder.Property(course => course.Title)
            .HasConversion(
                title => title.Value,
                value => new Title(value));

        builder.Property(course => course.Description)
            .HasConversion(
                description => description.Value,
                value => new Description(value));

        builder.Property(course => course.InstructorId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? new InstructorId(value.Value) : null);

        builder.Property(course => course.Status)
            .HasConversion<string>();

        builder.HasMany(course => course.Lessons)
            .WithOne()
            .HasForeignKey(lesson => lesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(course => course.Lessons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(course => course.Images, images =>
        {
            images.ToJson("course_images");
        });

        builder.OwnsOne(course => course.Price, price =>
        {
            price.Property(p => p.Amount).HasColumnName("price_amount").IsRequired();
            price.Property(p => p.Currency).HasColumnName("price_currency").IsRequired();
        });

        builder.HasQueryFilter(course => !course.IsDeleted);

        builder.Ignore(course => course.DomainEvents);
    }
}

using Domain.Courses;
using Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Courses;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(Course => Course.Id);

        builder.Property(course => course.Id)
            .HasConversion(
                id => id.Value,
                value => new CourseId(value));

        builder.Property(Course => Course.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(course => course.Lessons)
            .WithOne()
            .HasForeignKey(lesson => lesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(course => course.Lessons)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.OwnsOne(course => course.Price, price =>
        {
            price.Property(p => p.Amount).HasColumnName("PriceAmount").IsRequired();
            price.Property(p => p.Currency).HasColumnName("PriceCurrency").HasMaxLength(3).IsRequired();
        });
    }
}

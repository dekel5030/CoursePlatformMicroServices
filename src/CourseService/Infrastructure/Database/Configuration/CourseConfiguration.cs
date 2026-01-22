using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Configuration;

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
                        value => new Title(value))
                    .HasMaxLength(200);

        builder.Property(course => course.Description)
            .HasConversion(
                description => description.Value,
                value => new Description(value))
            .HasMaxLength(2000);

        builder.Property(course => course.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(course => course.Difficulty)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.OwnsOne(course => course.Price, price =>
        {
            price.Property(p => p.Amount).HasColumnName("PriceAmount").IsRequired();
            price.Property(p => p.Currency).HasColumnName("PriceCurrency").HasMaxLength(3).IsRequired();
        });

        builder.Property(course => course.Language)
            .HasConversion(language => language.Code, code => Language.Parse(code))
            .HasMaxLength(10);

        builder.Property(course => course.Slug)
            .HasConversion(
                slug => slug.Value,
                value => new Slug(value))
            .HasMaxLength(200);

        builder.HasIndex(course => course.Slug).IsUnique();

        builder.Property(course => course.InstructorId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(course => course.CategoryId)
            .HasConversion(id => id.Value, value => new CategoryId(value));

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(course => course.Images, imageBuilder =>
        {
            imageBuilder.ToJson("course_images");
        });

        builder.OwnsMany(course => course.Tags, tagBuilder =>
        {
            tagBuilder.ToJson("course_tags");
        });

        builder.HasQueryFilter(course => course.Status != CourseStatus.Deleted);

        builder.Ignore(course => course.DomainEvents);
    }
}

using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CourseHeaderConfiguration : IEntityTypeConfiguration<CourseHeaderReadModel>
{
    public void Configure(EntityTypeBuilder<CourseHeaderReadModel> builder)
    {
        builder.ToTable("course_headers", SchemaNames.Read);
        builder.HasKey(header => header.Id);

        builder.Property(header => header.Title).IsRequired();
        builder.Property(header => header.Description).IsRequired();
        builder.Property(header => header.InstructorName).IsRequired();
        builder.Property(header => header.Language).IsRequired();
        builder.Property(header => header.PriceCurrency).IsRequired();
        builder.Property(header => header.CategoryName).IsRequired();
        builder.Property(header => header.CategorySlug).IsRequired();

        builder.Property(header => header.ImageUrls)
            .HasColumnType("jsonb")
            .HasColumnName("image_urls");

        builder.Property(header => header.Tags)
            .HasColumnType("jsonb")
            .HasColumnName("tags");
    }
}

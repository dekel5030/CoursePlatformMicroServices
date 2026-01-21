using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Configuration;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");
        builder.HasKey(lesson => lesson.Id);
        builder.HasIndex(l => new { l.ModuleId, l.Index }).IsUnique();

        builder.Property(lesson => lesson.Id)
            .HasConversion(
                id => id.Value,
                value => new LessonId(value));

        builder.Property(lesson => lesson.Title)
            .HasConversion(
                title => title.Value,
                value => new Title(value))
            .HasMaxLength(200);

        builder.Property(lesson => lesson.Description)
            .HasConversion(
                description => description.Value,
                value => new Description(value))
            .HasMaxLength(2000);

        builder.Property(lesson => lesson.Slug)
            .HasConversion(
                slug => slug.Value,
                value => new Slug(value));

        builder.HasIndex(lesson => lesson.Slug).IsUnique();

        builder.Property(l => l.ThumbnailImageUrl)
            .HasConversion(
                url => url != null ? url.Path : null,
                value => value != null ? new ImageUrl(value) : null);

        builder.Property(lesson => lesson.VideoUrl)
            .HasConversion(
                url => url != null ? url.Path : null,
                value => value != null ? new VideoUrl(value) : null);

        builder.Property(lesson => lesson.Access)
            .HasConversion<string>();

        builder.OwnsMany(l => l.Attachments, attachmentBuilder =>
        {
            attachmentBuilder.ToJson("attachments");
        });
    }
}

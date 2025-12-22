using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Lessons;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");
        builder.HasKey(lesson => lesson.Id);

        builder.Property(lesson => lesson.Id)
            .HasConversion(
                id => id.Value,
                value => new LessonId(value));

        builder.Property(lesson => lesson.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(lesson => lesson.Course)
            .WithMany(course => course.Lessons)
            .HasForeignKey(lesson => lesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CourseStructureConfiguration : IEntityTypeConfiguration<CourseStructureReadModel>
{
    public void Configure(EntityTypeBuilder<CourseStructureReadModel> builder)
    {
        builder.ToTable("course_structures", SchemaNames.Read);
        builder.HasKey(structure => structure.CourseId);

        builder.OwnsMany(structure => structure.Modules, moduleBuilder =>
        {
            moduleBuilder.ToJson("modules");
            moduleBuilder.Property(module => module.Title).IsRequired();

            moduleBuilder.OwnsMany(module => module.Lessons, lessonBuilder =>
            {
                lessonBuilder.HasJsonPropertyName("lessons");
                lessonBuilder.Property(lesson => lesson.Title).IsRequired();
            });
        });
    }
}

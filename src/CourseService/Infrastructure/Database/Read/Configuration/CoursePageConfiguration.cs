using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CoursePageConfiguration : IEntityTypeConfiguration<CoursePage>
{
    public void Configure(EntityTypeBuilder<CoursePage> builder)
    {
        builder.ToTable("course_pages", SchemaNames.Read);
        builder.HasKey(coursePage => coursePage.Id);

        builder.OwnsMany(cp => cp.Modules, moduleBuilder =>
        {
            moduleBuilder.ToJson("modules");
            moduleBuilder.OwnsMany(m => m.Lessons, lessonBuilder =>
            {
                lessonBuilder.HasJsonPropertyName("lessons");
            });
        });
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.MediaProcessingTask;
using Courses.Domain.MediaProcessingTask.Primitives;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Write.Configuration;

public sealed class MediaProcessingTaskConfiguration : IEntityTypeConfiguration<MediaProcessingTask>
{
    public void Configure(EntityTypeBuilder<MediaProcessingTask> builder)
    {
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id)
            .HasConversion(id => id.Value, value => new TaskId(value));

        builder.Property(task => task.OriginalLessonId)
            .HasConversion(id => id.Value, value => new LessonId(value));

        builder.Property(task => task.AssignedTo)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder
            .HasOne<Lesson>()
            .WithOne()
            .HasForeignKey<MediaProcessingTask>(task => task.OriginalLessonId);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(task => task.AssignedTo);

        builder.OwnsMany(task => task.InputRawResources, builder =>
        {
            builder.ToJson();
        });

        builder.OwnsMany(task => task.OutputResources, builder =>
        {
            builder.ToJson();
        });

        builder.Property(task =>  task.Status)
            .HasConversion<string>();
    }
}

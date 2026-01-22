using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Configuration;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => new ModuleId(value));

        builder.Property(m => m.Title)
            .HasConversion(
                title => title.Value,
                value => new Title(value))
            .HasMaxLength(200);

        builder.Property(m => m.Index)
            .IsRequired();

        builder.Property(m => m.CourseId)
            .HasConversion(
                id => id.Value,
                value => new CourseId(value));

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(m => m.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Lessons)
            .WithOne()
            .HasForeignKey(l => l.ModuleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

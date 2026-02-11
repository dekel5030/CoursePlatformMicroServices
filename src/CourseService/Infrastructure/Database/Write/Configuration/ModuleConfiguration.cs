using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Write.Configuration;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules");

        builder.HasKey(m => m.Id);
        builder.HasIndex(m => new { m.CourseId, m.Index }).IsUnique();

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
    }
}

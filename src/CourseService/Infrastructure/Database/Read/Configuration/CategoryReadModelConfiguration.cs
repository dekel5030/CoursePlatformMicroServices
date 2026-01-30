using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database.Read.Configuration;

public sealed class CategoryReadModelConfiguration : IEntityTypeConfiguration<CategoryReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryReadModel> builder)
    {
        builder.ToTable("categories", SchemaNames.Read);
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(250);
    }
}

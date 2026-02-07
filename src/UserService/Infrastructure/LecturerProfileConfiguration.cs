using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;
using Users.Domain.Users.Primitives;

namespace Users.Infrastructure;

public class LecturerProfileConfiguration : IEntityTypeConfiguration<LecturerProfile>
{
    private const string TableName = "lecturer_profiles";
    private const string ProjectsTableName = "lecturer_projects";
    private const string MediaItemsTableName = "lecturer_media_items";
    private const string PostsTableName = "lecturer_posts";

    public void Configure(EntityTypeBuilder<LecturerProfile> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Id)
            .HasConversion(
                id => id.Value,
                value => new LecturerProfileId(value));

        builder.Property(lp => lp.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.HasIndex(lp => lp.UserId).IsUnique();

        builder.Property(lp => lp.ProfessionalBio).HasMaxLength(2000);
        builder.Property(lp => lp.Expertise).HasMaxLength(500);
        builder.Property(lp => lp.YearsOfExperience).HasDefaultValue(0);

        // Configure Projects collection
        builder.OwnsMany(lp => lp.Projects, projectBuilder =>
        {
            projectBuilder.ToTable(ProjectsTableName);
            projectBuilder.WithOwner().HasForeignKey("LecturerProfileId");
            projectBuilder.HasKey(nameof(Project.Id));
            projectBuilder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            projectBuilder.Property(p => p.Description).HasMaxLength(1000);
            projectBuilder.Property(p => p.Url).HasMaxLength(2048);
            projectBuilder.Property(p => p.ThumbnailUrl).HasMaxLength(2048);
            projectBuilder.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
        });

        // Configure MediaItems collection
        builder.OwnsMany(lp => lp.MediaItems, mediaBuilder =>
        {
            mediaBuilder.ToTable(MediaItemsTableName);
            mediaBuilder.WithOwner().HasForeignKey("LecturerProfileId");
            mediaBuilder.HasKey(nameof(MediaItem.Id));
            mediaBuilder.Property(m => m.Url).IsRequired().HasMaxLength(2048);
            mediaBuilder.Property(m => m.Title).HasMaxLength(200);
            mediaBuilder.Property(m => m.Description).HasMaxLength(500);
            mediaBuilder.Property(m => m.Type).HasConversion<string>();
            mediaBuilder.Property(m => m.UploadedAt).HasColumnType("timestamp without time zone");
        });

        // Configure Posts collection
        builder.OwnsMany(lp => lp.Posts, postBuilder =>
        {
            postBuilder.ToTable(PostsTableName);
            postBuilder.WithOwner().HasForeignKey("LecturerProfileId");
            postBuilder.HasKey(nameof(Post.Id));
            postBuilder.Property(p => p.Title).IsRequired().HasMaxLength(300);
            postBuilder.Property(p => p.Content).IsRequired();
            postBuilder.Property(p => p.ThumbnailUrl).HasMaxLength(2048);
            postBuilder.Property(p => p.PublishedAt).HasColumnType("timestamp without time zone");
            postBuilder.Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
        });
    }
}

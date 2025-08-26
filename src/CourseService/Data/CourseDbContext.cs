using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Data;

public class CourseDbContext : DbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options)
        : base(options) {}

    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureCourse(modelBuilder);
        ConfigureLesson(modelBuilder);

        modelBuilder.Entity<Enrollment>(enrollment =>
        {
            enrollment.HasKey(e => e.EnrollmentId);
            enrollment.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureCourse(ModelBuilder modelBuilder)
    {
        var course = modelBuilder.Entity<Course>();

        course.HasKey(c => c.Id);

        course.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(SchemaDefaults.TitleMaxLength)
            .HasDefaultValue(string.Empty);

        course.Property(c => c.Description)
            .HasMaxLength(SchemaDefaults.DescriptionMaxLength);

        course.Property(c => c.ImageUrl)
            .HasMaxLength(SchemaDefaults.UrlMaxLength);

        course.Property(c => c.InstructorUserId)
            .HasMaxLength(SchemaDefaults.IdMaxLength);

        course.Property(c => c.InstructorUserId)
            .HasMaxLength(SchemaDefaults.IdMaxLength);

        course.Property(c => c.IsPublished)
            .HasDefaultValue(false);

        course.Property(c => c.Price)
            .HasPrecision(SchemaDefaults.PricePrecision, SchemaDefaults.PriceScale);

        course.Property(c => c.CreatedAt)
            .HasDefaultValueSql("NOW()");

        course.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        course.HasMany(c => c.Lessons)
            .WithOne(l => l.Course)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureLesson(ModelBuilder modelBuilder)
    {
        var lesson = modelBuilder.Entity<Lesson>();

        lesson.HasKey(l => l.Id);

        lesson.HasIndex(l => new { l.CourseId, l.Order })
            .IsUnique();

        lesson.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(SchemaDefaults.TitleMaxLength)
            .HasDefaultValue(string.Empty);

        lesson.Property(l => l.Description)
            .HasMaxLength(SchemaDefaults.DescriptionMaxLength);

        lesson.Property(l => l.VideoUrl)
            .HasMaxLength(SchemaDefaults.UrlMaxLength);

        lesson.Property(l => l.ThumbnailUrl)
            .HasMaxLength(SchemaDefaults.UrlMaxLength);

        lesson.Property(l => l.IsPreview)
            .HasDefaultValue(false);

        lesson.Property(l => l.Order)
            .IsRequired()
            .HasDefaultValue(1);

        lesson.Property(l => l.Duration);

        lesson.Property(l => l.CourseId)
            .IsRequired();

        lesson.HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId);

        lesson.Property(l => l.CreatedAt)
            .HasDefaultValueSql("NOW()");

        lesson.Property(l => l.UpdatedAt)
            .HasDefaultValueSql("NOW()");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        foreach (var e in ChangeTracker.Entries<Enrollment>())
        {
            if (e.State == EntityState.Added)
            {
                e.Entity.AggregateVersion = 0;
            }
            else if (e.State == EntityState.Modified)
            {
                e.Entity.AggregateVersion++;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
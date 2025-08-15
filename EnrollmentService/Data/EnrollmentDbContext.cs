using System.Reflection;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data;

public class EnrollmentDbContext : DbContext
{
    public EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options)
        : base(options) {}

    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<KnownUser> KnownUsers { get; set; }
    public DbSet<KnownCourse> KnownCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<KnownCourse>().HasKey(x => x.CourseId);
        modelBuilder.Entity<KnownUser>().HasKey(x => x.UserId);

        base.OnModelCreating(modelBuilder);
    }
}
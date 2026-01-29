using Courses.Application.Abstractions.Data;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database;

public abstract class AppDbContextBase : DbContext, IUnitOfWork

{
    private bool _hasSaved;
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Module> Modules { get; set; }

    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_hasSaved)
        {
            //throw new InvalidOperationException(
            //    "SaveChangesAsync can only be called once per request. Use CommitAsync in the UnitOfWork instead.");
        }

        _hasSaved = true;
        return base.SaveChangesAsync(cancellationToken);
    }
}

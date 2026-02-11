using Courses.Application.Abstractions.Data;
using Courses.Application.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Ratings;
using Courses.Domain.Users;
using Courses.Infrastructure.Database.Write;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Read;

internal sealed class ReadDbContext : IReadDbContext
{
    private readonly WriteDbContext _context;

    public ReadDbContext(WriteDbContext context)
    {
        _context = context;
    }

    public IQueryable<Course> Courses => _context.Courses.AsNoTracking();
    public IQueryable<Lesson> Lessons => _context.Lessons.AsNoTracking();
    public IQueryable<Module> Modules => _context.Modules.AsNoTracking();
    public IQueryable<User> Users => _context.Users.AsNoTracking();
    public IQueryable<Category> Categories => _context.Categories.AsNoTracking();
    public IQueryable<Enrollment> Enrollments => _context.Enrollments.AsNoTracking();
    public IQueryable<CourseRating> CourseRatings => _context.CourseRatings.AsNoTracking();
    public IQueryable<CourseAnalytics> CourseAnalytics => _context.CourseAnalytics.AsNoTracking();
}

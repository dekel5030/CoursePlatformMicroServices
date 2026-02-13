using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Shared.Loaders;

internal sealed class CoursePageDataLoader : ICoursePageDataLoader
{
    private readonly IReadDbContext _dbContext;

    public CoursePageDataLoader(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CoursePageData?> LoadAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        var raw = await _dbContext.Courses
            .AsSplitQuery()
            .Where(c => c.Id == courseId)
            .Select(course => new
            {
                Course = course,
                Modules = _dbContext.Modules
                    .Where(m => m.CourseId == course.Id)
                    .OrderBy(m => m.Index)
                    .ToList(),
                Lessons = _dbContext.Lessons
                    .Where(l => l.CourseId == course.Id)
                    .OrderBy(l => l.Index)
                    .ToList(),
                Instructor = _dbContext.Users.FirstOrDefault(u => u.Id == course.InstructorId),
                Category = _dbContext.Categories.FirstOrDefault(cat => cat.Id == course.CategoryId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw == null)
        {
            return null;
        }

        return new CoursePageData(
            raw.Course,
            raw.Modules,
            raw.Lessons,
            raw.Instructor,
            raw.Category);
    }
}

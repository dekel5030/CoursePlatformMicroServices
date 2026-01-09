using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Repositories;

public class FeaturedCoursesRepo : IFeaturedCoursesRepository
{
    private readonly IReadDbContext _dbContext;

    public FeaturedCoursesRepo(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static readonly List<CourseId> _featuredCourseIds = new()
    {
        new CourseId(Guid.Parse("019a6842-b9a5-714a-b257-d164982cbc19")),
        new CourseId(Guid.Parse("019a6842-95e4-76bd-ac68-7884af14ae5f")),
        new CourseId(Guid.Parse("019a778e-4e98-7e5a-8e86-c2fc7b756f70")),
        new CourseId(Guid.Parse("019a778e-ea05-7b08-9308-f381eae6b750")),
    };

    public async Task<IReadOnlyList<Course>> GetFeaturedCourse()
    {
        List<Course> course = await _dbContext.Courses
            .Where(course => _featuredCourseIds.Contains(course.Id)).ToListAsync();

        return course.AsReadOnly();
    }
}
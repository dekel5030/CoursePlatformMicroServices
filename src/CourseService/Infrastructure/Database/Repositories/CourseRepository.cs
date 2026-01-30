using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Infrastructure.Database.Write;

namespace Courses.Infrastructure.Database.Repositories;

public class CourseRepository : RepositoryBase<Course, CourseId>, ICourseRepository
{
    public CourseRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}

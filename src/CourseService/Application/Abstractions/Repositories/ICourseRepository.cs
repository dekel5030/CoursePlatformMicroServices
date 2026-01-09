using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Repositories;

public interface ICourseRepository : IRepository<Course, CourseId>
{
    Task AddAsync(Course entity, CancellationToken cancellationToken = default);
}


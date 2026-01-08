using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Data.Repositories;

public interface ICourseRepository : IRepository<Course, CourseId>
{
    void Add(Course entity);
}


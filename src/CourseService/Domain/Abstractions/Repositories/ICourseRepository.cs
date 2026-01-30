using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface ICourseRepository : IRepository<Course, CourseId>;

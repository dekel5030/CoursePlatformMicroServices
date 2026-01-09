using Courses.Domain.Courses;

namespace Courses.Application.Abstractions.Repositories;

public interface IFeaturedCoursesRepository
{
    Task<IReadOnlyList<Course>> GetFeaturedCourse();
}

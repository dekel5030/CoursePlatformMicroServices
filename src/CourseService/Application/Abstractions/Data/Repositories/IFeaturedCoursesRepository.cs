using Courses.Domain.Courses;

namespace Courses.Application.Abstractions.Data.Repositories;

public interface IFeaturedCoursesRepository
{
    Task<IReadOnlyList<Course>> GetFeaturedCourse();
}

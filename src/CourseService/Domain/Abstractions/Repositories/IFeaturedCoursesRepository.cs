using Courses.Domain.Courses.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface IFeaturedCoursesRepository
{
    Task<IReadOnlyList<CourseId>> GetFeaturedCourseIds();
}

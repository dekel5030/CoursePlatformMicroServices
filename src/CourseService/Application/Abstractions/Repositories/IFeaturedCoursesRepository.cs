using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Repositories;

public interface IFeaturedCoursesRepository
{
    Task<IReadOnlyList<CourseId>> GetFeaturedCourseIds();
}

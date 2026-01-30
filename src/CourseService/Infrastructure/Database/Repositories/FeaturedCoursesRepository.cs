using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;

namespace Courses.Infrastructure.Database.Repositories;

public class FeaturedCoursesRepository : IFeaturedCoursesRepository
{
    private static readonly List<CourseId> _featuredCourseIds = new()
    {
        new CourseId(Guid.Parse("019be5ba-c9af-783c-b3f2-201a56e759d0")),
        new CourseId(Guid.Parse("019be5ba-9e07-7919-9998-c247772474c6")),
        new CourseId(Guid.Parse("019be5ba-5eac-78f8-97d1-7008fe4ae739")),
        new CourseId(Guid.Parse("019be5b9-7a87-72b6-86f8-9079d093cc37")),
    };

    public Task<IReadOnlyList<CourseId>> GetFeaturedCourseIds()
    {
        return Task.FromResult<IReadOnlyList<CourseId>>(_featuredCourseIds.AsReadOnly());
    }
}

using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Features.Shared.Loaders;

internal interface ICoursePageDataLoader
{
    Task<CoursePageData?> LoadAsync(CourseId courseId, CancellationToken cancellationToken = default);
}

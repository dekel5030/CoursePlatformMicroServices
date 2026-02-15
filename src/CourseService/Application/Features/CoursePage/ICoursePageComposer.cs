using Courses.Application.Features.Shared.Loaders;
using Courses.Application.ReadModels;

namespace Courses.Application.Features.CoursePage;

public interface ICoursePageComposer
{
    CoursePageDto Compose(CoursePageData data, CourseAnalytics? analytics);
}

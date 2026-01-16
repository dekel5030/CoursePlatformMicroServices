using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Policies.Courses;

public interface ICourseActionRule
{
    IEnumerable<CourseAction> Evaluate(Course course, IUserContext userContext);
}

using Courses.Application.Actions.Primitives;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Policies.Courses;

public interface ICourseActionRule
{
    IEnumerable<CourseAction> Evaluate(CoursePolicyContext context, IUserContext userContext);
}

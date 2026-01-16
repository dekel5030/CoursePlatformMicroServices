using Courses.Application.Actions.Primitives;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Policies.CourseCollection;

public interface ICourseCollectionActionRule
{
    IEnumerable<CourseCollectionAction> Evaluate(IUserContext userContext);
}

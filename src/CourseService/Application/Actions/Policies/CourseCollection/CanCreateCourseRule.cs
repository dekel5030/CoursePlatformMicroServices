using Courses.Application.Actions.Primitives;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.CourseCollection;

internal sealed class CanCreateCourseRule : ICourseCollectionActionRule
{
    public IEnumerable<CourseCollectionAction> Evaluate(IUserContext userContext)
    {
        if (userContext.HasPermission(ActionType.Create, ResourceType.Course, ResourceId.Wildcard))
        {
            yield return CourseCollectionAction.CreateCourse;
        }
    }
}

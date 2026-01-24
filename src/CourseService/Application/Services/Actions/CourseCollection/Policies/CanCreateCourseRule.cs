using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Services.Actions.CourseCollection.Policies;

internal sealed class CanCreateCourseRule : ICourseCollectionActionRule
{
    public IEnumerable<CourseCollectionAction> Evaluate(Empty state, IUserContext userContext)
    {
        if (userContext.HasPermission(ActionType.Create, ResourceType.Course, ResourceId.Wildcard))
        {
            yield return CourseCollectionAction.CreateCourse;
        }
    }
}

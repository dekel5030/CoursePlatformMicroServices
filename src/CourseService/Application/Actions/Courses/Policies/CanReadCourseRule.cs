using Courses.Domain.Courses.Primitives;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Courses.Policies;

internal sealed class CanReadCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(CourseState state, IUserContext userContext)
    {
        var resourceId = ResourceId.Create(state.CourseId.Value.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Read, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Read;
            yield break;
        }

        if (state.Status == CourseStatus.Published)
        {
            yield return CourseAction.Read;
        }
    }
}

using Courses.Domain.Courses;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Courses.Policies;

internal sealed class CanDeleteCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(CourseState state, IUserContext userContext)
    {
        Result canDeleteResult = CoursePolicies.CanDelete(state.Status);

        if (canDeleteResult.IsFailure)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(state.CourseId.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Delete, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Delete;
        }
    }
}
